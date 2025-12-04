using Microsoft.EntityFrameworkCore;
using MyCommerce.Application.Common.Interfaces;
using MyCommerce.Application.Dashboard.Dtos;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Application.Dashboard;

public class DashboardService
{
    private readonly IAppDbContext _context;

    public DashboardService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        // 1. Total Revenue (Sum of all orders)
        // Note: In a real app, consider payment status. Here we sum all placed orders.
        var totalRevenue = await _context.Orders.SumAsync(o => o.Total.Amount, cancellationToken);

        // 2. Counts
        var totalOrders = await _context.Orders.CountAsync(cancellationToken);
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var totalUsers = await _context.Users.CountAsync(cancellationToken);

        // 3. Low Stock Products (Threshold < 10)
        var lowStockProducts = await _context.Products
            .Where(p => p.Stock < 10)
            .OrderBy(p => p.Stock)
            .Take(5)
            .Select(p => new LowStockProductDto(p.Id, p.Name, p.Stock))
            .ToListAsync(cancellationToken);

        // 4. Recent Orders (Last 5)
        var recentOrdersEntities = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync(cancellationToken);

        // Get Emails for recent orders
        var userIds = recentOrdersEntities.Select(o => o.UserId).Distinct();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email.Value, cancellationToken);

        var recentOrders = recentOrdersEntities.Select(o => new RecentOrderDto(
            o.Id,
            users.GetValueOrDefault(o.UserId, "Unknown"),
            o.Total.Amount,
            o.OrderDate,
            o.Status
        )).ToList();

        return new DashboardStatsDto(
            totalRevenue,
            totalOrders,
            totalProducts,
            totalUsers,
            lowStockProducts,
            recentOrders);
    }
}
