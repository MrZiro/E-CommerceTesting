using Mapster;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Application.Categories.Dtos;

public class CategoryDto : IRegister
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid? ParentId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>();
    }
}
