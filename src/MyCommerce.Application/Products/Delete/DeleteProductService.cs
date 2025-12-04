using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Products.Delete;

public class DeleteProductService
{
    private readonly IAppDbContext _context;

    public DeleteProductService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<None>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product is null)
        {
             return Result.Fail<None>(DomainErrors.Product.NotFound);
        }

        // Check dependencies: Do not allow deletion if the product is part of any order
        var isOrdered = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id, cancellationToken);
        if (isOrdered)
        {
            return Result.Fail<None>(DomainErrors.Product.CannotDeleteInUse);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success(None.Value);
    }
}
