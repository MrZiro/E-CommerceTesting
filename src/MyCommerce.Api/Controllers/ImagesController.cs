using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Common.Interfaces;

namespace MyCommerce.Api.Controllers;

public class ImagesController : ApiController
{
    private readonly IFileStorage _fileStorage;

    public ImagesController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Optional: Add validation for file type (image/png, image/jpeg) and size

        using var stream = file.OpenReadStream();
        var url = await _fileStorage.SaveFileAsync(stream, file.FileName, cancellationToken);

        return Ok(new { Url = url });
    }
}
