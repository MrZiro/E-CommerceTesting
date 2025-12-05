using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MyCommerce.Application.Authentication;
using MyCommerce.Application.Products.Create;
using MyCommerce.Application.Products.Queries.GetAllProducts;
using MyCommerce.Application.Products.Queries.GetProductById;
using MyCommerce.Application.Categories.Create;
using MyCommerce.Application.Categories.Delete;
using MyCommerce.Application.Categories.Queries.GetAllCategories;
using MyCommerce.Application.Categories.Queries.GetCategoryById;
using MyCommerce.Application.Categories.Update;
using MyCommerce.Application.Carts;
using MyCommerce.Application.Orders;
using MyCommerce.Application.Dashboard;
using MyCommerce.Application.Products.Update;
using MyCommerce.Application.Products.Delete;

namespace MyCommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        AddMapping(services, assembly);
        AddServices(services);

        return services;
    }

    private static void AddMapping(IServiceCollection services, Assembly assembly)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        
        services.AddScoped<CreateProductService>();
        services.AddScoped<UpdateProductService>();
        services.AddScoped<DeleteProductService>();
        services.AddScoped<GetAllProductsService>();
        services.AddScoped<GetProductByIdService>();
        
        // Categories
        services.AddScoped<CreateCategoryService>();
        services.AddScoped<GetAllCategoriesService>();
        services.AddScoped<GetCategoryByIdService>();
        services.AddScoped<UpdateCategoryService>();
        services.AddScoped<DeleteCategoryService>();
        
        services.AddScoped<CartService>();
        services.AddScoped<OrderService>();
        services.AddScoped<DashboardService>();
        
        // Users
        services.AddScoped<MyCommerce.Application.Users.Queries.GetUserById.GetUserByIdService>();
        services.AddScoped<MyCommerce.Application.Users.Update.UpdateProfile.UpdateUserProfileService>();
    }
}