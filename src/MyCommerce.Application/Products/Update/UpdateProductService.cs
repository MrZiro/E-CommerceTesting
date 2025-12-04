using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Products.Update;

public class UpdateProductService
{
    private readonly IAppDbContext _context;

    public UpdateProductService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<None>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);

        if (product is null)
        {
            return Result.Fail<None>(new Error("Product.NotFound", "Product not found."));
        }

        // Validate Category
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category is null)
        {
            return Result.Fail<None>(new Error("Product.InvalidCategory", "Category not found."));
        }

        // Create Value Object
        var priceResult = Money.From(request.PriceAmount, request.Currency);
        if (priceResult.IsFailure)
        {
            return Result.Fail<None>(priceResult.Errors);
        }

        // Update Entity
        var updateResult = product.UpdateDetails(
            request.Name,
            request.Description,
            priceResult.Value,
            request.Stock,
            request.CategoryId,
            request.ImageUrl);

        if (updateResult.IsFailure)
        {
            return Result.Fail<None>(updateResult.Errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(None.Value);
    }
}
