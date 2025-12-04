using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Authentication;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : ApiController
{
    private readonly AuthService _authService;

    public UsersController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.CreateUserAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(new { Id = result.Value });
    }
}
