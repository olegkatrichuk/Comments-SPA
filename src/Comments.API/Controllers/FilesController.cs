using Comments.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comments.API.Controllers;

[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile(string fileName, CancellationToken ct = default)
    {
        var result = await _fileStorageService.GetAsync(fileName, ct);

        if (result is null)
            return NotFound();

        var (fileStream, contentType, originalFileName) = result.Value;
        return File(fileStream, contentType, originalFileName);
    }
}
