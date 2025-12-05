namespace MyCommerce.Application.Dashboard.Dtos;

public record DashboardStatsDto(
    decimal TotalRevenue,
    int TotalOrders,
    int TotalProducts,
    int TotalUsers,
    List<LowStockProductDto> LowStockProducts,
    List<RecentOrderDto> RecentOrders);

public record LowStockProductDto(
    Guid Id,
    string Name,
    int Stock);

public record RecentOrderDto(
    Guid Id,
    string UserEmail,
    decimal TotalAmount,
    DateTime Date,
    string Status);
