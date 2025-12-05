using MyCommerce.Domain.Common;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Domain.Entities;

public sealed class Category : AggregateRoot
{
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; } // For hierarchical categories

    // Private constructor for EF Core
    private Category() 
    {
        Name = null!;
    }

    private Category(Guid id, string name, Guid? parentId)
        : base(id)
    {
        Name = name;
        ParentId = parentId;
    }

    public static Result<Category> Create(string name, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Assuming a DomainError for empty category name exists or creating one
            return Result.Fail<Category>(new Error("Category.EmptyName", "Category name cannot be empty."));
        }

        return new Category(Guid.NewGuid(), name, parentId);
    }

    public Result<None> UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Fail<None>(new Error("Category.EmptyName", "Category name cannot be empty."));
        }
        Name = newName;
        return Result.Success(None.Value);
    }

    public Result<None> SetParent(Guid? parentId)
    {
        ParentId = parentId;
        return Result.Success(None.Value);
    }
}