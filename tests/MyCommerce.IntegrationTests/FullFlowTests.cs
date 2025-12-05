using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using MyCommerce.Application.Authentication;
using MyCommerce.Application.Carts.Dtos;
using MyCommerce.Application.Categories.Create;
using MyCommerce.Application.Orders.Dtos;
using MyCommerce.Application.Products.Create;
using MyCommerce.Domain.Entities;

namespace MyCommerce.IntegrationTests;

public class FullFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FullFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullECommerceFlow_ShouldWork()
    {
        // 1. Login as Admin (Seeded)
        var adminLoginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            "admin@mycommerce.com", "Admin123!"));
        
        adminLoginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var adminAuth = await adminLoginResponse.Content.ReadFromJsonAsync<AuthResult>();
        var adminToken = adminAuth!.Token;

        // 2. Create Category (As Admin)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("Electronics", null));
        categoryResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

        // 3. Create Product (As Admin)
        var uniqueSku = $"IPHONE-{Guid.NewGuid().ToString().Substring(0, 8)}";
        var productRequest = new CreateProductRequest(
            "iPhone 15", "Cool phone", 999, "USD", uniqueSku, 100, categoryId, null);
        
        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);
        
        if (productResponse.StatusCode != System.Net.HttpStatusCode.Created)
        {
            var error = await productResponse.Content.ReadAsStringAsync();
            throw new Exception($"Create Product Failed: {error}");
        }

        productResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var productId = await productResponse.Content.ReadFromJsonAsync<Guid>();

        // 4. Register Customer
        var uniqueEmail = $"john-{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
        var registerRequest = new RegisterRequest("John", "Doe", uniqueEmail, "Customer123!");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        
        if (registerResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
             var error = await registerResponse.Content.ReadAsStringAsync();
             throw new Exception($"Register Failed: {error}");
        }

        registerResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // 5. Login as Customer
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(uniqueEmail, "Customer123!"));
        loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var customerAuth = await loginResponse.Content.ReadFromJsonAsync<AuthResult>();
        var customerToken = customerAuth!.Token;

        // 6. Add to Cart (As Customer)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", customerToken);
        var addToCartResponse = await _client.PostAsJsonAsync("/api/cart/items", new { ProductId = productId, Quantity = 1 });
        addToCartResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        // 7. Checkout
        var checkoutResponse = await _client.PostAsync("/api/orders/checkout", null);
        checkoutResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var checkoutResult = await checkoutResponse.Content.ReadFromJsonAsync<CheckoutResult>();
        checkoutResult!.OrderId.Should().NotBeEmpty();

        // 8. Verify Order
        var myOrdersResponse = await _client.GetAsync("/api/orders/my-orders");
        myOrdersResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var orders = await myOrdersResponse.Content.ReadFromJsonAsync<List<OrderDto>>();
        
        orders.Should().ContainSingle();
        orders!.First().TotalAmount.Should().Be(999);
        orders!.First().Status.Should().Be("Pending");
    }

    // Helper record for parsing checkout response (anonymous object in controller)
    public record CheckoutResult(Guid OrderId, string Message);
}
