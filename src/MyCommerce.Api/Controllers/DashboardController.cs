using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Dashboard;

namespace MyCommerce.Api.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : ApiController
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetStatsAsync(cancellationToken);
        return Ok(result.Value);
    }
}
