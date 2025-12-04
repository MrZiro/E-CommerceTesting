using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;

namespace MyCommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(m => m.Currency)
                .HasMaxLength(3);
            priceBuilder.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)");
        });

        builder.OwnsOne(p => p.Sku, skuBuilder =>
        {
            skuBuilder.Property(s => s.Value)
                .HasMaxLength(50)
                .HasColumnName("Sku");
            
            skuBuilder.HasIndex(s => s.Value).IsUnique();
        });

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId);
    }
}
