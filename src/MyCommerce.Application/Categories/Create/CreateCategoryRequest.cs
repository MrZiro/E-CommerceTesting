namespace MyCommerce.Application.Categories.Create;

public record CreateCategoryRequest(string Name, Guid? ParentId);
