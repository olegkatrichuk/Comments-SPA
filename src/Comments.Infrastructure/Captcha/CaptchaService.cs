using Comments.Domain.Interfaces;
using SkiaSharp;
using StackExchange.Redis;

namespace Comments.Infrastructure.Captcha;

public sealed class CaptchaService : ICaptchaService
{
    private const int ImageWidth = 180;
    private const int ImageHeight = 60;
    private const int MinCaptchaLength = 5;
    private const int MaxCaptchaLength = 6;
    private const string CaptchaChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private static readonly TimeSpan CaptchaTtl = TimeSpan.FromMinutes(5);
    private const string CacheKeyPrefix = "captcha:";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CaptchaService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<(string Key, byte[] ImageBytes)> GenerateAsync(CancellationToken ct = default)
    {
        var captchaText = GenerateRandomText();
        var captchaKey = $"{CacheKeyPrefix}{Guid.NewGuid():N}";

        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(captchaKey, captchaText, CaptchaTtl);

        var imageBytes = RenderCaptchaImage(captchaText);

        return (captchaKey, imageBytes);
    }

    public async Task<bool> ValidateAsync(string key, string answer, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(answer))
            return false;

        var db = _connectionMultiplexer.GetDatabase();
        var storedAnswer = await db.StringGetAsync(key);

        if (storedAnswer.IsNullOrEmpty)
            return false;

        // Remove the key after validation (one-time use)
        await db.KeyDeleteAsync(key);

        return string.Equals(storedAnswer, answer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string GenerateRandomText()
    {
        var random = Random.Shared;
        var length = random.Next(MinCaptchaLength, MaxCaptchaLength + 1);
        return string.Create(length, random, (span, rng) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = CaptchaChars[rng.Next(CaptchaChars.Length)];
            }
        });
    }

    private static byte[] RenderCaptchaImage(string text)
    {
        using var surface = SKSurface.Create(new SKImageInfo(ImageWidth, ImageHeight));
        var canvas = surface.Canvas;

        // Background
        var bgColor = new SKColor(
            (byte)Random.Shared.Next(200, 240),
            (byte)Random.Shared.Next(200, 240),
            (byte)Random.Shared.Next(200, 240));
        canvas.Clear(bgColor);

        // Noise lines
        using var noisePaint = new SKPaint
        {
            IsAntialias = true,
            StrokeWidth = 1.5f,
            Style = SKPaintStyle.Stroke
        };

        for (var i = 0; i < 8; i++)
        {
            noisePaint.Color = new SKColor(
                (byte)Random.Shared.Next(100, 200),
                (byte)Random.Shared.Next(100, 200),
                (byte)Random.Shared.Next(100, 200));

            canvas.DrawLine(
                Random.Shared.Next(0, ImageWidth),
                Random.Shared.Next(0, ImageHeight),
                Random.Shared.Next(0, ImageWidth),
                Random.Shared.Next(0, ImageHeight),
                noisePaint);
        }

        // Draw characters with rotation
        using var typeface = SKTypeface.FromFamilyName("DejaVu Sans", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
                             ?? SKTypeface.Default;
        using var font = new SKFont(typeface, 32);
        using var textPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var charSpacing = (ImageWidth - 20) / text.Length;

        for (var i = 0; i < text.Length; i++)
        {
            textPaint.Color = new SKColor(
                (byte)Random.Shared.Next(0, 100),
                (byte)Random.Shared.Next(0, 100),
                (byte)Random.Shared.Next(0, 100));

            var x = 10 + i * charSpacing;
            var y = ImageHeight / 2 + 10 + Random.Shared.Next(-5, 6);
            var rotation = Random.Shared.Next(-25, 26);

            canvas.Save();
            canvas.RotateDegrees(rotation, x, y);
            canvas.DrawText(text[i].ToString(), x, y, font, textPaint);
            canvas.Restore();
        }

        // Additional noise dots
        using var dotPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        for (var i = 0; i < 50; i++)
        {
            dotPaint.Color = new SKColor(
                (byte)Random.Shared.Next(0, 255),
                (byte)Random.Shared.Next(0, 255),
                (byte)Random.Shared.Next(0, 255),
                (byte)Random.Shared.Next(50, 150));

            canvas.DrawCircle(
                Random.Shared.Next(0, ImageWidth),
                Random.Shared.Next(0, ImageHeight),
                Random.Shared.Next(1, 3),
                dotPaint);
        }

        canvas.Flush();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }
}
