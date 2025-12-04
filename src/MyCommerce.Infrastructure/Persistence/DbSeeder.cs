using MyCommerce.Application.Common.Interfaces.Authentication;
using MyCommerce.Domain.Entities;
using MyCommerce.Domain.ValueObjects;

namespace MyCommerce.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        // Ensure database is created (optional, usually migrations handle this, but good for dev)
        // await context.Database.EnsureCreatedAsync(); 

        if (!context.Users.Any())
        {
            var emailResult = Email.From("admin@mycommerce.com");
            if (emailResult.IsFailure) return; // Should not happen

            var userResult = User.Create(
                "Admin",
                "User",
                emailResult.Value,
                passwordHasher.HashPassword("Admin123!"),
                new List<string> { "Admin", "Customer" }
            );

            if (userResult.IsSuccess)
            {
                context.Users.Add(userResult.Value);
                await context.SaveChangesAsync();
            }
        }
    }
}
