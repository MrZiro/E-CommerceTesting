namespace MyCommerce.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null);
