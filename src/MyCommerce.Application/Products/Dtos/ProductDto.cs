using Mapster;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;

namespace MyCommerce.Application.Products.Dtos;

public class ProductDto : IRegister
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public int Stock { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; init; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.PriceAmount, src => src.Price.Amount)
            .Map(dest => dest.PriceCurrency, src => src.Price.Currency)
            .Map(dest => dest.Sku, src => src.Sku.Value);
    }
}
