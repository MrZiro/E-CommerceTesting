using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Domain.Entities;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; } = null!; // Set through factory method
    public Sku Sku { get; private set; } = null!; // Set through factory method
    public int Stock { get; private set; }
    public Guid CategoryId { get; private set; } // Foreign key
    public string? ImageUrl { get; private set; }

    // Private constructor for EF Core
    private Product() 
    {
        Name = null!;
        Description = null!;
        Price = null!;
        Sku = null!;
    }

    private Product(
        Guid id,
        string name,
        string description,
        Money price,
        Sku sku,
        int stock,
        Guid categoryId,
        string? imageUrl)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        Stock = stock;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
    }

    public static Result<Product> Create(
        string name,
        string description,
        Money price,
        Sku sku,
        int stock,
        Guid categoryId,
        string? imageUrl = null)
    {
        // Basic validations
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        {
            return Result.Fail<Product>(DomainErrors.Product.NameTooLong);
        }

        if (price.Amount <= 0)
        {
            return Result.Fail<Product>(DomainErrors.Product.InvalidPrice);
        }
        
        // Sku and Money have their own validation in their factory methods.
        // We assume price and sku are already valid here.

        return new Product(
            Guid.NewGuid(),
            name,
            description,
            price,
            sku,
            stock,
            categoryId,
            imageUrl);
    }

    // Methods for behavior (e.g., ChangePrice, UpdateStock)
    public Result<None> UpdateStock(int quantityChange)
    {
        if (Stock + quantityChange < 0)
        {
            return Result.Fail<None>(DomainErrors.Product.InvalidStockChange); 
        }

        Stock += quantityChange;
        // AddDomainEvent(new ProductStockUpdatedEvent(Id, Stock));
        return Result.Success(None.Value);
    }

    public Result<None> UpdateDetails(
        string name,
        string description,
        Money price,
        int stock,
        Guid categoryId,
        string? imageUrl)
    {
         if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
        {
            return Result.Fail<None>(DomainErrors.Product.NameTooLong);
        }

        if (price.Amount <= 0)
        {
            return Result.Fail<None>(DomainErrors.Product.InvalidPrice);
        }

        if (stock < 0)
        {
             return Result.Fail<None>(DomainErrors.Product.InvalidStockChange);
        }

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        
        return Result.Success(None.Value);
    }
}