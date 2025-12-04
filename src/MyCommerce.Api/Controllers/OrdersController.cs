using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Orders;

namespace MyCommerce.Api.Controllers;

[Authorize]
public class OrdersController : ApiController
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                      ?? User.FindFirst("sub"); 
        return idClaim != null ? Guid.Parse(idClaim.Value) : Guid.Empty;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _orderService.PlaceOrderAsync(userId, cancellationToken: cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(new { OrderId = result.Value, Message = "Order placed successfully." });
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _orderService.GetMyOrdersAsync(userId, cancellationToken);
        
        return Ok(result.Value);
    }
}
