using System.Threading.RateLimiting;
using Serilog;
using MyCommerce.Application;
using MyCommerce.Infrastructure;
using MyCommerce.Infrastructure.Persistence;
using MyCommerce.Application.Common.Interfaces.Authentication;
using MyCommerce.Infrastructure.Services.Storage;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    builder.Host.UseSerilog(); // Use Serilog for hosting logs

    // Configure Storage Settings
    builder.Services.Configure<StorageSettings>(settings =>
    {
        var webRootPath = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
        if (!Directory.Exists(webRootPath))
        {
            Directory.CreateDirectory(webRootPath);
        }
        settings.BasePath = webRootPath;
        settings.BaseUrl = ""; // Relative URL
    });

    // Add services to the container.
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    
    builder.Services.AddMemoryCache(); // Add In-Memory Cache

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
            };
        });
    
    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: token);
        };

        // Global Policy: 100 requests per minute per IP
        options.AddFixedWindowLimiter(policyName: "Global", options =>
        {
            options.PermitLimit = 100;
            options.Window = TimeSpan.FromMinutes(1);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 5; // Allow small burst
        });

        // Auth Policy: 10 requests per minute per IP (Brute force protection)
        options.AddFixedWindowLimiter(policyName: "Auth", options =>
        {
            options.PermitLimit = 10;
            options.Window = TimeSpan.FromMinutes(1);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 0;
        });
    });

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddControllers(); // Add Controllers service
    builder.Services.AddEndpointsApiExplorer(); // Needed for Scalar UI
    builder.Services.AddHealthChecks(); // Add Health Checks

    builder.Services.AddExceptionHandler<MyCommerce.Api.Middleware.GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder => builder.WithOrigins("http://localhost:3000") // Replace with your frontend URL
                                .AllowAnyHeader()
                                .AllowAnyMethod());
    });

    var app = builder.Build();

    // Seed Database
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DbSeeder.SeedAsync(context, hasher, app.Environment.IsDevelopment());
    }

    app.UseExceptionHandler(); // Add the exception handling middleware
    app.UseSerilogRequestLogging(); // Add Serilog request logging middleware
    app.UseCors("AllowSpecificOrigin"); // Use CORS middleware
    app.UseRateLimiter(); // Enable Rate Limiting
    app.UseDefaultFiles(); // Enable default files mapping
    app.UseStaticFiles(); // Serve static files from wwwroot

    app.UseAuthentication();
    app.UseAuthorization();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(); // Add Scalar UI
    }

    app.UseHttpsRedirection();

    app.MapHealthChecks("/health"); // Map Health Checks
    app.MapControllers(); // Map Controllers

    if (app.Environment.IsDevelopment())
    {
        Log.Information("==========================================================");
        Log.Information("🚀 Server is running!");
        Log.Information("👉 App:   http://localhost:5006");
        Log.Information("👉 Docs:  http://localhost:5006/scalar/v1");
        Log.Information("==========================================================");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
