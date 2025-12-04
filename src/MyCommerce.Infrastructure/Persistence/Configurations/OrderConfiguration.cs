using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCommerce.Domain.Entities;

namespace MyCommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(o => o.Total, totalBuilder =>
        {
            totalBuilder.Property(m => m.Currency).HasMaxLength(3);
            totalBuilder.Property(m => m.Amount).HasColumnType("decimal(18,2)");
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
