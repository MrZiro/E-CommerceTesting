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

        var allowedTypes = new[] { "image/png", "image/jpeg", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
        {
            return BadRequest("Only image files (PNG, JPEG, GIF, WebP) are allowed.");
        }

        const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxSizeBytes)
        {
            return BadRequest("File size exceeds the 5 MB limit.");
        }

        using var stream = file.OpenReadStream();
        var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var url = await _fileStorage.SaveFileAsync(stream, safeFileName, cancellationToken);

        return Ok(new { Url = url });
    }
}
