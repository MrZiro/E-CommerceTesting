using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Categories.Create;

public class CreateCategoryService
{
    private readonly IAppDbContext _context;

    public CreateCategoryService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
             return Result.Fail<Guid>(new Error("Category.EmptyName", "Name is required."));
        }

        // Check if ParentId exists if provided
        if (request.ParentId.HasValue)
        {
            var parentExists = await _context.Categories.FindAsync(new object[] { request.ParentId.Value }, cancellationToken);
            if (parentExists is null)
            {
                return Result.Fail<Guid>(new Error("Category.InvalidParent", "Parent category does not exist."));
            }
        }

        var categoryResult = Category.Create(request.Name, request.ParentId);

        if (categoryResult.IsFailure)
        {
            return Result.Fail<Guid>(categoryResult.Errors);
        }

        _context.Categories.Add(categoryResult.Value);
        await _context.SaveChangesAsync(cancellationToken);

        return categoryResult.Value.Id;
    }
}
