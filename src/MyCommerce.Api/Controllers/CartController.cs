using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Carts;
using MyCommerce.Application.Carts.Dtos;

namespace MyCommerce.Api.Controllers;

[Authorize]
public class CartController : ApiController
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    private Guid GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                      ?? User.FindFirst("sub"); // JWT standard 'sub'
        
        return idClaim != null ? Guid.Parse(idClaim.Value) : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _cartService.GetCartAsync(userId, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }
        return Ok(result.Value);
    }

    [HttpPut("items/{productId}")]
    public async Task<IActionResult> UpdateItem(Guid productId, [FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _cartService.UpdateQuantityAsync(userId, productId, request.Quantity, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }
        return Ok(result.Value);
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _cartService.RemoveFromCartAsync(userId, productId, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }
        return Ok(result.Value);
    }
}

public record AddCartItemRequest(Guid ProductId, int Quantity);
public record UpdateCartItemRequest(int Quantity);
