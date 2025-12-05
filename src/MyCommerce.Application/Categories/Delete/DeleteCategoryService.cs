using FluentValidation;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Categories.Delete;

public class DeleteCategoryService
{
    private readonly IAppDbContext _context;

    public DeleteCategoryService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);

        if (category is null)
        {
            return Result.Fail(new Error("Category.NotFound", "The category with the specified ID was not found."));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
