using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Products.Dtos;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Products.Queries.GetAllProducts;

public class GetAllProductsService
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllProductsService(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ProductDto>>> GetAllAsync(GetAllProductsQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ProductDto>>(products);
    }
}
