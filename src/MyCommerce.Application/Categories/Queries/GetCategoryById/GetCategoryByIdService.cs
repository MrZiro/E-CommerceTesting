using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Categories.Dtos;
using MyCommerce.Domain.Common.Result;
using MyCommerce.Domain.Errors;

namespace MyCommerce.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdService
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryByIdService(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Fail<CategoryDto>(new Error("Category.NotFound", "Category not found."));
        }

        return _mapper.Map<CategoryDto>(category);
    }
}
