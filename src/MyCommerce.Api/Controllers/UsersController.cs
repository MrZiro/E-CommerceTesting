using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Authentication;
using MyCommerce.Application.Users.Queries.GetUserById;
using MyCommerce.Application.Users.Update.UpdateProfile;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

[Authorize]
public class UsersController : ApiController
{
    private readonly AuthService _authService;
    private readonly GetUserByIdService _getUserByIdService;
    private readonly UpdateUserProfileService _updateUserProfileService;

    public UsersController(
        AuthService authService,
        GetUserByIdService getUserByIdService,
        UpdateUserProfileService updateUserProfileService)
    {
        _authService = authService;
        _getUserByIdService = getUserByIdService;
        _updateUserProfileService = updateUserProfileService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.CreateUserAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(new { Id = result.Value });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _getUserByIdService.GetByIdAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _updateUserProfileService.UpdateAsync(userId, request, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return NoContent();
    }

    private Guid GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                      ?? User.FindFirst("sub");
        
        if (idClaim is null || !Guid.TryParse(idClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }
        return userId;
    }
}