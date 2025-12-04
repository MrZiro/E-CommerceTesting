using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);

        builder.OwnsOne(oi => oi.UnitPrice, priceBuilder =>
        {
            priceBuilder.Property(m => m.Currency).HasMaxLength(3);
            priceBuilder.Property(m => m.Amount).HasColumnType("decimal(18,2)");
        });
    }
}
