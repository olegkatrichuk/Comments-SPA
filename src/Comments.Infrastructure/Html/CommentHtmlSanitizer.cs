using Ganss.Xss;
using IAppHtmlSanitizer = Comments.Application.Services.IHtmlSanitizer;

namespace Comments.Infrastructure.Html;

public sealed class CommentHtmlSanitizer : IAppHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer;

    public CommentHtmlSanitizer()
    {
        _sanitizer = new HtmlSanitizer();

        // Clear all defaults
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedCssProperties.Clear();
        _sanitizer.AllowedAtRules.Clear();

        // Allow only specific tags
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("code");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("strong");

        // Allow only href and title on <a> tags
        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("title");

        // Allow http and https schemes for links
        _sanitizer.AllowedSchemes.Add("http");
        _sanitizer.AllowedSchemes.Add("https");
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        return _sanitizer.Sanitize(html);
    }
}
