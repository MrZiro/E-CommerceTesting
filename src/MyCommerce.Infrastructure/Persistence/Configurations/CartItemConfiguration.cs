using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);
        
        // Optional: Add foreign key to Product if you want DB-level consistency
        // builder.HasOne<Product>().WithMany().HasForeignKey(ci => ci.ProductId);
    }
}
