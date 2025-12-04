using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors; // Assuming more general errors might be defined here

namespace MyCommerce.Domain.Entities;

public sealed class Order : AggregateRoot
{
    public Guid UserId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Money Total { get; private set; } = null!; // Set through factory method
    public string Status { get; private set; } // e.g., "Pending", "Processing", "Shipped", "Delivered", "Cancelled"

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Private constructor for EF Core
    private Order() 
    {
        Total = null!;
        Status = null!;
    }

    private Order(
        Guid id,
        Guid userId,
        DateTime orderDate,
        Money total,
        string status,
        IEnumerable<OrderItem> orderItems)
        : base(id)
    {
        UserId = userId;
        OrderDate = orderDate;
        Total = total;
        Status = status;
        _orderItems.AddRange(orderItems);
    }

    public static Result<Order> Create(
        Guid userId,
        IEnumerable<OrderItem> orderItems,
        string status = "Pending")
    {
        if (userId == Guid.Empty)
        {
            return Result.Fail<Order>(new Error("Order.EmptyUserId", "User ID cannot be empty."));
        }

        if (orderItems == null || !orderItems.Any())
        {
            return Result.Fail<Order>(new Error("Order.NoItems", "Order must contain at least one item."));
        }

        // Calculate total from order items
        var totalAmount = orderItems.Sum(item => item.Quantity * item.UnitPrice.Amount);
        var currency = orderItems.First().UnitPrice.Currency; // Assuming all items have the same currency

        var totalResult = Money.From(totalAmount, currency);

        if (totalResult.IsFailure)
        {
            return Result.Fail<Order>(totalResult.Errors);
        }

        return new Order(
            Guid.NewGuid(),
            userId,
            DateTime.UtcNow,
            totalResult.Value,
            status,
            orderItems);
    }

    public Result<None> ChangeStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            return Result.Fail<None>(new Error("Order.EmptyStatus", "Order status cannot be empty."));
        }
        Status = newStatus;
        // Consider adding domain event here: AddDomainEvent(new OrderStatusChangedEvent(Id, newStatus));
        return Result.Success(None.Value);
    }

    // Example of adding an item to an existing order (may have business rules)
    public Result<None> AddItem(OrderItem newItem)
    {
        // Check for existing item or add new
        var existingItem = _orderItems.FirstOrDefault(oi => oi.ProductId == newItem.ProductId);
        if (existingItem != null)
        {
            // Update quantity, or return error if not allowed to update
            return Result.Fail<None>(new Error("Order.DuplicateItem", "Item already exists in order."));
        }

        _orderItems.Add(newItem);
        // Recalculate total
        var newTotalAmount = OrderItems.Sum(item => item.Quantity * item.UnitPrice.Amount);
        var currency = OrderItems.First().UnitPrice.Currency;
        var newTotalResult = Money.From(newTotalAmount, currency);

        if (newTotalResult.IsFailure)
        {
            return Result.Fail<None>(newTotalResult.Errors);
        }
        Total = newTotalResult.Value;

        // AddDomainEvent(new OrderItemAddedEvent(Id, newItem.Id));
        return Result.Success(None.Value);
    }
}