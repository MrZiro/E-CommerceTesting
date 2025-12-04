using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Domain.Entities;

public sealed class CartItem : Entity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    private CartItem() { }

    private CartItem(Guid id, Guid cartId, Guid productId, int quantity) : base(id)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }

    public static Result<CartItem> Create(Guid cartId, Guid productId, int quantity)
    {
        if (cartId == Guid.Empty)
            return Result.Fail<CartItem>(DomainErrors.CartItem.EmptyCartId);
        if (productId == Guid.Empty)
            return Result.Fail<CartItem>(DomainErrors.CartItem.EmptyProductId);
        if (quantity <= 0)
            return Result.Fail<CartItem>(DomainErrors.CartItem.InvalidQuantity);
        
        return new CartItem(Guid.NewGuid(), cartId, productId, quantity);
    }

    public Result AddQuantity(int quantity)
    {
        if (Quantity + quantity <= 0)
            return Result.Fail(new Error("CartItem.InvalidQuantity", "Resulting quantity must be greater than zero."));
        Quantity += quantity;
        return Result.Ok();
    }

    public Result SetQuantity(int quantity)
    {
        if (quantity <= 0)
            return Result.Fail(DomainErrors.CartItem.InvalidQuantity);
        Quantity = quantity;
        return Result.Ok();
    }
}
