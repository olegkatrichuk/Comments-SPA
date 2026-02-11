using FluentValidation;

namespace Comments.Application.Comments.Commands.CreateComment;

public sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".gif", ".png"];
    private static readonly string[] AllowedTextExtensions = [".txt"];
    private const long MaxTextFileSizeBytes = 100 * 1024; // 100 KB

    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required.")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("User name must contain only latin letters, digits, and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.HomePage)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                         (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .When(x => !string.IsNullOrWhiteSpace(x.HomePage))
            .WithMessage("Home page must be a valid HTTP or HTTPS URL.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text is required.");

        RuleFor(x => x.CaptchaKey)
            .NotEmpty().WithMessage("CAPTCHA key is required.");

        RuleFor(x => x.CaptchaAnswer)
            .NotEmpty().WithMessage("CAPTCHA answer is required.");

        RuleFor(x => x.FileName)
            .Must(fileName =>
            {
                var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
                return AllowedImageExtensions.Contains(extension) ||
                       AllowedTextExtensions.Contains(extension);
            })
            .When(x => !string.IsNullOrWhiteSpace(x.FileName))
            .WithMessage("Allowed file types are: .jpg, .gif, .png, .txt.");

        RuleFor(x => x.FileStream)
            .Must((command, stream) =>
            {
                if (stream is null || string.IsNullOrWhiteSpace(command.FileName))
                    return true;

                var extension = Path.GetExtension(command.FileName)?.ToLowerInvariant();
                if (extension == ".txt" && stream.Length > MaxTextFileSizeBytes)
                    return false;

                return true;
            })
            .When(x => x.FileStream is not null)
            .WithMessage($"Text files must not exceed {MaxTextFileSizeBytes / 1024} KB.");
    }
}
