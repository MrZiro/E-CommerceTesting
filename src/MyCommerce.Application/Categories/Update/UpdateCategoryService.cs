using FluentValidation;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using Microsoft.EntityFrameworkCore;

namespace MyCommerce.Application.Categories.Update;

public class UpdateCategoryService
{
    private readonly IAppDbContext _context;

    public UpdateCategoryService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);

        if (category is null)
        {
            return Result.Fail(new Error("Category.NotFound", "The category with the specified ID was not found."));
        }

        var nameResult = category.UpdateName(request.Name);
        if (nameResult.IsFailure)
        {
            return Result.Fail(nameResult.Errors);
        }

        var parentResult = category.SetParent(request.ParentId);
        if (parentResult.IsFailure)
        {
            return Result.Fail(parentResult.Errors);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}