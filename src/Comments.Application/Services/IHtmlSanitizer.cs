namespace Comments.Application.Services;

/// <summary>
/// Sanitizes HTML content, allowing only safe tags:
/// &lt;a href="" title=""&gt;, &lt;code&gt;, &lt;i&gt;, &lt;strong&gt;.
/// All other tags and attributes are stripped.
/// </summary>
public interface IHtmlSanitizer
{
    string Sanitize(string html);
}
