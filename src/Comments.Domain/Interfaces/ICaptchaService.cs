namespace Comments.Domain.Interfaces;

public interface ICaptchaService
{
    Task<(string Key, byte[] ImageBytes)> GenerateAsync(CancellationToken ct = default);
    Task<bool> ValidateAsync(string key, string answer, CancellationToken ct = default);
}
