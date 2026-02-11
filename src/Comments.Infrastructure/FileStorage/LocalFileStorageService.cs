using Comments.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Comments.Infrastructure.FileStorage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private const int MaxImageWidth = 320;
    private const int MaxImageHeight = 240;
    private const long MaxTextFileSizeBytes = 100 * 1024; // 100 KB

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".gif", ".png"
    };

    private static readonly HashSet<string> AllowedTextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt"
    };

    private static readonly Dictionary<string, string> ContentTypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".png", "image/png" },
        { ".txt", "text/plain" }
    };

    private readonly string _uploadsPath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _uploadsPath = configuration["FileStorage:UploadsPath"]
            ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        _logger = logger;

        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<(string StoredFileName, string ContentType, long FileSize)> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("File must have an extension.", nameof(fileName));

        var isImage = AllowedImageExtensions.Contains(extension);
        var isText = AllowedTextExtensions.Contains(extension);

        if (!isImage && !isText)
            throw new ArgumentException(
                $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedImageExtensions.Concat(AllowedTextExtensions))}",
                nameof(fileName));

        if (isText)
        {
            if (fileStream.CanSeek && fileStream.Length > MaxTextFileSizeBytes)
                throw new ArgumentException(
                    $"Text file exceeds the maximum size of {MaxTextFileSizeBytes / 1024} KB.",
                    nameof(fileStream));
        }

        var resolvedContentType = ContentTypeMappings.GetValueOrDefault(extension, contentType);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(_uploadsPath, storedFileName);
        long fileSize;

        if (isImage)
        {
            fileSize = await SaveImageAsync(fileStream, filePath, extension, ct);
        }
        else
        {
            fileSize = await SaveTextFileAsync(fileStream, filePath, ct);
        }

        _logger.LogInformation(
            "Saved file '{FileName}' as '{StoredFileName}' ({FileSize} bytes)",
            fileName, storedFileName, fileSize);

        return (storedFileName, resolvedContentType, fileSize);
    }

    public Task<(Stream FileStream, string ContentType, string FileName)?> GetAsync(
        string storedFileName,
        CancellationToken ct = default)
    {
        var filePath = Path.Combine(_uploadsPath, storedFileName);

        if (!File.Exists(filePath))
            return Task.FromResult<(Stream FileStream, string ContentType, string FileName)?>(null);

        var extension = Path.GetExtension(storedFileName);
        var contentType = ContentTypeMappings.GetValueOrDefault(extension, "application/octet-stream");
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return Task.FromResult<(Stream FileStream, string ContentType, string FileName)?>(
            (stream, contentType, storedFileName));
    }

    public Task DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_uploadsPath, storedFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("Deleted file '{StoredFileName}'", storedFileName);
        }

        return Task.CompletedTask;
    }

    private static async Task<long> SaveImageAsync(
        Stream fileStream,
        string filePath,
        string extension,
        CancellationToken ct)
    {
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;

        using var original = SKBitmap.Decode(memoryStream);

        if (original is null)
            throw new ArgumentException("The file is not a valid image.");

        if (original.Width > MaxImageWidth || original.Height > MaxImageHeight)
        {
            var ratioX = (float)MaxImageWidth / original.Width;
            var ratioY = (float)MaxImageHeight / original.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(original.Width * ratio);
            var newHeight = (int)(original.Height * ratio);

            using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);

            if (resized is null)
                throw new InvalidOperationException("Failed to resize the image.");

            using var image = SKImage.FromBitmap(resized);
            var format = GetSkiaFormat(extension);
            using var data = image.Encode(format, 90);

            await using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            data.SaveTo(outputStream);

            return new FileInfo(filePath).Length;
        }
        else
        {
            memoryStream.Position = 0;
            await using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await memoryStream.CopyToAsync(outputStream, ct);

            return new FileInfo(filePath).Length;
        }
    }

    private static async Task<long> SaveTextFileAsync(
        Stream fileStream,
        string filePath,
        CancellationToken ct)
    {
        await using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        long totalBytes = 0;
        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await fileStream.ReadAsync(buffer, ct)) > 0)
        {
            totalBytes += bytesRead;

            if (totalBytes > MaxTextFileSizeBytes)
            {
                outputStream.Close();

                if (File.Exists(filePath))
                    File.Delete(filePath);

                throw new ArgumentException(
                    $"Text file exceeds the maximum size of {MaxTextFileSizeBytes / 1024} KB.");
            }

            await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
        }

        return totalBytes;
    }

    private static SKEncodedImageFormat GetSkiaFormat(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            ".png" => SKEncodedImageFormat.Png,
            ".gif" => SKEncodedImageFormat.Gif,
            _ => SKEncodedImageFormat.Jpeg
        };
    }
}
