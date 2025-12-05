namespace MyCommerce.Application.Categories.Update;

public record UpdateCategoryRequest(string Name, Guid? ParentId);
