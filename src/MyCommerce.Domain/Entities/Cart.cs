using MyCommerce.Domain.Common;

namespace MyCommerce.Domain.Entities;

public sealed class Cart : AggregateRoot
{
    public Guid UserId { get; private set; }
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { } // EF Core

    private Cart(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }

    public static Cart Create(Guid userId)
    {
        return new Cart(Guid.NewGuid(), userId);
    }

    public void AddItem(Guid productId, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            _items.Add(CartItem.Create(Id, productId, quantity));
        }
    }

    public void RemoveItem(Guid productId)
    {
         var item = _items.FirstOrDefault(i => i.ProductId == productId);
         if (item != null)
         {
             _items.Remove(item);
         }
    }
    
    public void UpdateItemQuantity(Guid productId, int quantity)
    {
         var item = _items.FirstOrDefault(i => i.ProductId == productId);
         if (item != null)
         {
            if (quantity <= 0)
            {
                _items.Remove(item);
            }
            else
            {
                item.SetQuantity(quantity);
            }
         }
    }

    public void Clear()
    {
        _items.Clear();
    }
}
