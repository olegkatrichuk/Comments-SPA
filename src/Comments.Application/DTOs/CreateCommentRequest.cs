namespace Comments.Application.DTOs;

public sealed record CreateCommentRequest(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid? ParentCommentId,
    string CaptchaKey,
    string CaptchaAnswer);
