namespace Comments.Domain.Interfaces;

public interface IFileStorageService
{
    Task<(string StoredFileName, string ContentType, long FileSize)> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken ct = default);

    Task<(Stream FileStream, string ContentType, string FileName)?> GetAsync(
        string storedFileName,
        CancellationToken ct = default);

    Task DeleteAsync(string storedFileName, CancellationToken ct = default);
}
