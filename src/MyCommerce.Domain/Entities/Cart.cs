using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;

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

    public Result AddItem(Guid productId, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            return existingItem.AddQuantity(quantity);
        }
        else
        {
            var itemResult = CartItem.Create(Id, productId, quantity);
            if (itemResult.IsFailure)
            {
                return Result.Fail(itemResult.Errors);
            }
            _items.Add(itemResult.Value);
            return Result.Success();
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
