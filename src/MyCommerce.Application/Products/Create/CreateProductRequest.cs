namespace MyCommerce.Application.Products.Create;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal PriceAmount,
    string Currency,
    string Sku,
    int Stock,
    Guid CategoryId,
    string? ImageUrl);
