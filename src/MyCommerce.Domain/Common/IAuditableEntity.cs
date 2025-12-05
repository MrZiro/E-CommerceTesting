namespace MyCommerce.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; set; }
    DateTime? UpdatedOnUtc { get; set; }
}
