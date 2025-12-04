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

    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
    };

    public static Result<Order> Create(
        Guid userId,
        IEnumerable<OrderItem> orderItems,
        string status = "Pending")
    {
        if (userId == Guid.Empty)
        {
            return Result.Fail<Order>(DomainErrors.Order.EmptyUserId);
        }

        var itemsList = orderItems?.ToList() ?? new List<OrderItem>();
        if (itemsList.Count == 0)
        {
            return Result.Fail<Order>(DomainErrors.Order.NoItems);
        }

        // Validate currency consistency
        var currencies = itemsList.Select(i => i.UnitPrice.Currency).Distinct().ToList();
        if (currencies.Count > 1)
        {
            return Result.Fail<Order>(DomainErrors.Order.MixedCurrencies);
        }

        // Calculate total from order items
        var totalAmount = itemsList.Sum(item => item.Quantity * item.UnitPrice.Amount);
        var currency = currencies[0];

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
            itemsList);
    }

    public Result<None> ChangeStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            return Result.Fail<None>(DomainErrors.Order.EmptyStatus);
        }

        if (!ValidStatuses.Contains(newStatus))
        {
            return Result.Fail<None>(DomainErrors.Order.InvalidStatus);
        }

        Status = newStatus;
        // Consider adding domain event here: AddDomainEvent(new OrderStatusChangedEvent(Id, newStatus));
        return Result.Success(None.Value);
    }

    // Example of adding an item to an existing order (may have business rules)
    public Result<None> AddItem(OrderItem newItem)
    {
        // Validate currency consistency with existing items
        if (_orderItems.Any() && _orderItems.First().UnitPrice.Currency != newItem.UnitPrice.Currency)
        {
            return Result.Fail<None>(DomainErrors.Order.CurrencyMismatch);
        }

        // Check for existing item or add new
        var existingItem = _orderItems.FirstOrDefault(oi => oi.ProductId == newItem.ProductId);
        if (existingItem != null)
        {
            // Update quantity, or return error if not allowed to update
            return Result.Fail<None>(DomainErrors.Order.DuplicateItem);
        }

        // Recalculate total
        var newTotalAmount = _orderItems.Sum(item => item.Quantity * item.UnitPrice.Amount) + (newItem.Quantity * newItem.UnitPrice.Amount);
        var currency = newItem.UnitPrice.Currency; // Validated above
        var newTotalResult = Money.From(newTotalAmount, currency);

        if (newTotalResult.IsFailure)
        {
            return Result.Fail<None>(newTotalResult.Errors);
        }

        _orderItems.Add(newItem);
        Total = newTotalResult.Value;

        // AddDomainEvent(new OrderItemAddedEvent(Id, newItem.Id));
        return Result.Success(None.Value);
    }
}