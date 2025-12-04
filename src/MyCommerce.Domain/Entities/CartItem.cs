using MyCommerce.Domain.Common;

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

    public static CartItem Create(Guid cartId, Guid productId, int quantity)
    {
        return new CartItem(Guid.NewGuid(), cartId, productId, quantity);
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
