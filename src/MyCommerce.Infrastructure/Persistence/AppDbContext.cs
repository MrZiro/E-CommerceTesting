using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Domain.Entities;
using MyCommerce.Infrastructure.Persistence.Interceptors;

namespace MyCommerce.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
