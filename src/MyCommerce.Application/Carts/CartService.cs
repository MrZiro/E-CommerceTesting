using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Carts.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Carts;

public class CartService
{
    private readonly IAppDbContext _context;

    public CartService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CartDto>> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            // Return empty cart structure
            return new CartDto(Guid.Empty, userId, new List<CartItemDto>(), 0, "USD");
        }

        return await MapToDtoAsync(cart, cancellationToken);
    }

    public async Task<Result<CartDto>> AddToCartAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product is null)
        {
             return Result.Fail<CartDto>(new Error("Cart.InvalidProduct", "Product not found."));
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = Cart.Create(userId);
            _context.Carts.Add(cart);
        }

        cart.AddItem(productId, quantity);
        
        await _context.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(cart, cancellationToken);
    }

    public async Task<Result<CartDto>> RemoveFromCartAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
             return Result.Fail<CartDto>(new Error("Cart.NotFound", "Cart not found."));
        }

        cart.RemoveItem(productId);
        await _context.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(cart, cancellationToken);
    }
    
    public async Task<Result<CartDto>> UpdateQuantityAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

         if (cart is null)
        {
             return Result.Fail<CartDto>(new Error("Cart.NotFound", "Cart not found."));
        }
        
        cart.UpdateItemQuantity(productId, quantity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return await MapToDtoAsync(cart, cancellationToken);
    }

    private async Task<CartDto> MapToDtoAsync(Cart cart, CancellationToken cancellationToken)
    {
        // Need to fetch product details for the items
        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        var itemDtos = new List<CartItemDto>();
        decimal totalAmount = 0;
        string currency = "USD"; // Default

        foreach (var item in cart.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                var totalPrice = product.Price.Amount * item.Quantity;
                totalAmount += totalPrice;
                currency = product.Price.Currency;

                itemDtos.Add(new CartItemDto(
                    product.Id,
                    product.Name,
                    product.ImageUrl ?? "",
                    product.Price.Amount,
                    product.Price.Currency,
                    item.Quantity,
                    totalPrice
                ));
            }
        }

        return new CartDto(cart.Id, cart.UserId, itemDtos, totalAmount, currency);
    }
}
