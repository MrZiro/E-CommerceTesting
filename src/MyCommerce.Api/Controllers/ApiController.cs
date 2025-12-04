using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("Global")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        // Simple mapping of first error to status code
        // In real app, map Error Code to Status Code
        var firstError = errors[0];
        
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: firstError.Code,
            detail: firstError.Description);
    }
}
