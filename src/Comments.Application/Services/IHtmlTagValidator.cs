namespace Comments.Application.Services;

/// <summary>
/// Validates that HTML tags in the input are properly closed (XHTML-style).
/// Ensures every opened tag has a corresponding closing tag.
/// </summary>
public interface IHtmlTagValidator
{
    (bool IsValid, string? Error) ValidateTags(string html);
}
