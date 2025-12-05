using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Products.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Products.Queries.GetProductById;

public class GetProductByIdService
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdService(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> GetByIdAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .AsNoTracking() // Read-only query, no need to track changes
            .FirstOrDefaultAsync(p => p.Id == query.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Fail<ProductDto>(new Error("Product.NotFound", "Product not found."));
        }

        return _mapper.Map<ProductDto>(product);
    }
}
