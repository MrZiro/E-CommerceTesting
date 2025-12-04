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
        var productsQuery = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            productsQuery = productsQuery.Where(p => p.Name.Contains(query.SearchTerm));

        if (query.CategoryId.HasValue)
            productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);

        if (query.MinPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price.Amount >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price.Amount <= query.MaxPrice.Value);

        var products = await productsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ProductDto>>(products);
    }
}
