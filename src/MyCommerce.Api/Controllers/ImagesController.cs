using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Common.Interfaces;

namespace MyCommerce.Api.Controllers;

public class ImagesController : ApiController
{
    private readonly IFileStorage _fileStorage;

    private static readonly Dictionary<string, byte[][]> ImageSignatures = new()
    {
        { "image/png", new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
        { "image/jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { "image/gif", new[] { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { "image/webp", new[] { new byte[] { 0x52, 0x49, 0x46, 0x46 } } } // RIFF header
    };

    public ImagesController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    private static bool IsValidImageSignature(Stream stream, string contentType)
    {
        if (!ImageSignatures.TryGetValue(contentType, out var signatures))
            return false;
        
        var headerBytes = new byte[8];
        var bytesRead = stream.Read(headerBytes, 0, headerBytes.Length);
        stream.Position = 0; // Reset for subsequent read

        if (bytesRead == 0)
        {
            return false;
        }
        
        return signatures.Any(sig => 
            bytesRead >= sig.Length && 
            headerBytes.Take(sig.Length).SequenceEqual(sig));
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (!ImageSignatures.ContainsKey(file.ContentType))
        {
            return BadRequest("Only image files (PNG, JPEG, GIF, WebP) are allowed.");
        }

        const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxSizeBytes)
        {
            return BadRequest("File size exceeds the 5 MB limit.");
        }

        using var stream = file.OpenReadStream();

        // Validate magic bytes
        if (!IsValidImageSignature(stream, file.ContentType))
        {
             return BadRequest("Invalid file content. Signature verification failed.");
        }

        var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var url = await _fileStorage.SaveFileAsync(stream, safeFileName, cancellationToken);

        return Ok(new { Url = url });
    }
}
