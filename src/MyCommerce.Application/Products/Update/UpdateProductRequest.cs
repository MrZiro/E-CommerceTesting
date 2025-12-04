namespace MyCommerce.Application.Products.Update;

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal PriceAmount,
    string Currency,
    int Stock,
    Guid CategoryId,
    string? ImageUrl);
