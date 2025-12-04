using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCommerce.Infrastructure.Persistence;
using MyCommerce.Infrastructure.Persistence.Interceptors;

using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Common.Interfaces.Authentication;
using MyCommerce.Infrastructure.Authentication;
using MyCommerce.Infrastructure.Services.Storage;
using MyCommerce.Infrastructure.Services;

using MyCommerce.Infrastructure.Services.Payments;

namespace MyCommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        services.AddTransient<IEmailService, SmtpEmailService>();
        
        // Payment Strategies
        services.AddTransient<IPaymentStrategy, StripePaymentStrategy>();
        services.AddTransient<IPaymentStrategy, PayPalPaymentStrategy>();
        // Main Payment Service (Resolver)
        services.AddTransient<IPaymentService, PaymentService>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("MyCommerceDb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
