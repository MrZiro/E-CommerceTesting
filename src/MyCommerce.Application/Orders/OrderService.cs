using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Common.Models;
using MyCommerce.Application.Orders.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.Errors;
using MyCommerce.Domain.ValueObjects;

namespace MyCommerce.Application.Orders;

public class OrderService
{
    private readonly IAppDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;

    public OrderService(
        IAppDbContext context,
        IPaymentService paymentService,
        IEmailService emailService)
    {
        _context = context;
        _paymentService = paymentService;
        _emailService = emailService;
    }

    public async Task<Result<Guid>> PlaceOrderAsync(Guid userId, string paymentProvider = "stripe", CancellationToken cancellationToken = default)
    {
        // 1. Get Cart
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || !cart.Items.Any())
        {
            return Result.Fail<Guid>(new Error("Order.EmptyCart", "Cannot place order with empty cart."));
        }

        // 2. Get Products to check stock and get current prices
        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        // 3. Validate Stock and Create OrderItems
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.Items)
        {
            if (!products.TryGetValue(cartItem.ProductId, out var product))
            {
                return Result.Fail<Guid>(new Error("Order.ProductNotFound", $"Product with ID {cartItem.ProductId} not found."));
            }

            // Check Stock
            if (product.Stock < cartItem.Quantity)
            {
                return Result.Fail<Guid>(new Error("Order.OutOfStock", $"Not enough stock for product '{product.Name}'. Available: {product.Stock}, Requested: {cartItem.Quantity}"));
            }

            // Create OrderItem
            // Create new Money instance to avoid EF Core tracking issues with shared Value Objects
            var unitPriceResult  = Money.From(product.Price.Amount, product.Price.Currency);
            if (unitPriceResult.IsFailure)
            {
                return Result.Fail<Guid>(unitPriceResult.Errors);
            }
            
            var unitPrice = unitPriceResult.Value;
            var orderItemResult = OrderItem.Create(product.Id, cartItem.Quantity, unitPrice);
            
            if (orderItemResult.IsFailure)
            {
                return Result.Fail<Guid>(orderItemResult.Errors);
            }
            orderItems.Add(orderItemResult.Value);
        }

        // 4. Create Order Entity
        var orderResult = Order.Create(userId, orderItems);
        if (orderResult.IsFailure)
        {
            return Result.Fail<Guid>(orderResult.Errors);
        }

        var order = orderResult.Value;

        // 5. Process Payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(userId, order.Total.Amount, order.Total.Currency, paymentProvider, cancellationToken);
        if (paymentResult.IsFailure || !paymentResult.Value)
        {
            return Result.Fail<Guid>(new Error("Order.PaymentFailed", "Payment processing failed."));
        }

        // 6. Deduct Stock
        foreach (var item in orderItems)
        {
            var product = products[item.ProductId];
            var stockResult = product.UpdateStock(-item.Quantity);
            if (stockResult.IsFailure)
            {
                // Should not happen due to check above, but technically race condition possible if not locked
                return Result.Fail<Guid>(stockResult.Errors);
            }
        }

        // 7. Save Order
        _context.Orders.Add(order);

        // 8. Clear Cart
        // Option A: Clear items
        cart.Clear();
        // Option B: Delete Cart completely
        // _context.Carts.Remove(cart); 

        await _context.SaveChangesAsync(cancellationToken);

        // 9. Send Email
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            // We assume a SendOrderConfirmationAsync method exists or we construct a generic email
             // For this demo, we just use existing infrastructure but ideally we add a method to IEmailService
        }

        return order.Id;
    }

    public async Task<Result<List<OrderDto>>> GetMyOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

        // Fetch products
        var productIds = orders.SelectMany(o => o.OrderItems).Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var orderDtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            var itemDtos = order.OrderItems.Select(i => new OrderItemDto(
                i.ProductId,
                products.GetValueOrDefault(i.ProductId, "Unknown Product"),
                i.Quantity,
                i.UnitPrice.Amount
            )).ToList();

            orderDtos.Add(new OrderDto(
                order.Id,
                order.UserId,
                null, // Email not needed for self-view usually, or fetch if needed
                order.OrderDate,
                order.Total.Amount,
                order.Status,
                itemDtos
            ));
        }

        return orderDtos;
    }

    public async Task<Result<PagedResult<OrderDto>>> GetAllOrdersAsync(int pageNumber, int pageSize, string? status, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            return Result.Fail<PagedResult<OrderDto>>(new Error("Pagination.InvalidPageNumber", "Page number must be greater than or equal to 1."));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return Result.Fail<PagedResult<OrderDto>>(new Error("Pagination.InvalidPageSize", "Page size must be between 1 and 100."));
        }

        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Fetch User Emails and Product Names
        var userIds = orders.Select(o => o.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email.Value, cancellationToken);

        var productIds = orders.SelectMany(o => o.OrderItems).Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var orderDtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            var itemDtos = order.OrderItems.Select(i => new OrderItemDto(
                i.ProductId,
                products.GetValueOrDefault(i.ProductId, "Unknown Product"),
                i.Quantity,
                i.UnitPrice.Amount
            )).ToList();

            orderDtos.Add(new OrderDto(
                order.Id,
                order.UserId,
                users.GetValueOrDefault(order.UserId),
                order.OrderDate,
                order.Total.Amount,
                order.Status,
                itemDtos
            ));
        }

        return new PagedResult<OrderDto>(orderDtos, totalCount, pageNumber, pageSize);
    }

    public async Task<Result<None>> UpdateOrderStatusAsync(Guid orderId, string newStatus, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);

        if (order is null)
        {
            return Result.Fail<None>(new Error("Order.NotFound", "Order not found."));
        }

        var result = order.ChangeStatus(newStatus);
        if (result.IsFailure)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(None.Value);
    }
}
