using MyCommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace MyCommerce.Infrastructure.Services.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly StorageSettings _settings;

    public LocalFileStorage(IOptions<StorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadsFolder = Path.Combine(_settings.BasePath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Sanitize filename
        var sanitizedFileName = Path.GetFileName(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream, cancellationToken);
        }

        // Return relative URL
        return $"{_settings.BaseUrl}/uploads/{uniqueFileName}";
    }
}
