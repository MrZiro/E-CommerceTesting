using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Domain.Entities;

public sealed class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!; // Set through factory method

    // Private constructor for EF Core
    private OrderItem() 
    {
        UnitPrice = null!;
    }

    private OrderItem(
        Guid id,
        Guid productId,
        int quantity,
        Money unitPrice)
        : base(id)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static Result<OrderItem> Create(
        Guid productId,
        int quantity,
        Money unitPrice)
    {
        if (productId == Guid.Empty)
        {
            return Result.Fail<OrderItem>(new Error("OrderItem.EmptyProductId", "Product ID cannot be empty."));
        }
        if (quantity <= 0)
        {
            return Result.Fail<OrderItem>(new Error("OrderItem.InvalidQuantity", "Quantity must be greater than zero."));
        }
        // UnitPrice is expected to be valid as it's a Value Object

        return new OrderItem(
            Guid.NewGuid(),
            productId,
            quantity,
            unitPrice);
    }
}