using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Categories.Dtos;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesService
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllCategoriesService(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<CategoryDto>>> GetAllAsync(GetAllCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CategoryDto>>(categories);
    }
}
