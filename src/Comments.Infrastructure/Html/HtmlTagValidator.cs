using System.Text.RegularExpressions;
using Comments.Application.Services;

namespace Comments.Infrastructure.Html;

public sealed partial class HtmlTagValidator : IHtmlTagValidator
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "code", "i", "strong"
    };

    public (bool IsValid, string? Error) ValidateTags(string html)
    {
        if (string.IsNullOrEmpty(html))
            return (true, null);

        var matches = HtmlTagRegex().Matches(html);
        var openTags = new Stack<string>();

        foreach (Match match in matches)
        {
            var isClosing = match.Groups[1].Value == "/";
            var tagName = match.Groups[2].Value.ToLowerInvariant();
            var isSelfClosing = match.Groups[3].Value == "/";

            // Skip tags that are not in the allowed list
            if (!AllowedTags.Contains(tagName))
                continue;

            if (isSelfClosing)
                continue;

            if (isClosing)
            {
                if (openTags.Count == 0 || !string.Equals(openTags.Peek(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, $"Unexpected closing tag '</{tagName}>'. Expected closing tag for '{(openTags.Count > 0 ? openTags.Peek() : "none")}'.");
                }

                openTags.Pop();
            }
            else
            {
                openTags.Push(tagName);
            }
        }

        if (openTags.Count > 0)
        {
            var unclosedTags = string.Join(", ", openTags.Select(t => $"<{t}>"));
            return (false, $"Unclosed HTML tags: {unclosedTags}. All allowed tags must be properly closed.");
        }

        return (true, null);
    }

    [GeneratedRegex(@"<(/?)(\w+)(?:\s[^>]*)?(/?)\s*>", RegexOptions.Compiled)]
    private static partial Regex HtmlTagRegex();
}
