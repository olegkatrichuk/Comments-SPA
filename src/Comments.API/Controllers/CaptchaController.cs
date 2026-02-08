using Comments.Application.DTOs;
using Comments.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comments.API.Controllers;

[ApiController]
[Route("api/captcha")]
public sealed class CaptchaController : ControllerBase
{
    private readonly ICaptchaService _captchaService;

    public CaptchaController(ICaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    [HttpGet]
    public async Task<ActionResult<CaptchaDto>> GetCaptcha(CancellationToken ct = default)
    {
        var (key, imageBytes) = await _captchaService.GenerateAsync(ct);
        var imageBase64 = Convert.ToBase64String(imageBytes);
        var captcha = new CaptchaDto(key, imageBase64);
        return Ok(captcha);
    }
}
