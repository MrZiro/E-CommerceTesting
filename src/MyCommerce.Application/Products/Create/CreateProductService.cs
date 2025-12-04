using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Products.Create;

public class CreateProductService
{
    private readonly IAppDbContext _context;

    public CreateProductService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Validate Category exists
        var category = await _context.Categories
            .FindAsync(new object[] { request.CategoryId }, cancellationToken);

        if (category is null)
        {
            return Result.Fail<Guid>(new Error("Product.InvalidCategory", "Category does not exist."));
        }

        // 2. Check SKU uniqueness
        var skuResult = Sku.From(request.Sku);
        if (skuResult.IsFailure)
        {
            return Result.Fail<Guid>(skuResult.Errors);
        }

        var skuExists = await _context.Products
            .AnyAsync(p => p.Sku.Value == request.Sku, cancellationToken);

        if (skuExists)
        {
            return Result.Fail<Guid>(new Error("Product.DuplicateSku", "SKU already exists."));
        }

        // 3. Create Value Objects
        var priceResult = Money.From(request.PriceAmount, request.Currency);
        if (priceResult.IsFailure)
        {
            return Result.Fail<Guid>(priceResult.Errors);
        }

        // 4. Create Entity
        var productResult = Product.Create(
            request.Name,
            request.Description,
            priceResult.Value,
            skuResult.Value,
            request.Stock,
            request.CategoryId,
            request.ImageUrl);

        if (productResult.IsFailure)
        {
            return Result.Fail<Guid>(productResult.Errors);
        }

        // 5. Save
        _context.Products.Add(productResult.Value);
        await _context.SaveChangesAsync(cancellationToken);

        return productResult.Value.Id;
    }
}
