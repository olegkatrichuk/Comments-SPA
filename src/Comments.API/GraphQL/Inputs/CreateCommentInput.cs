namespace Comments.API.GraphQL.Inputs;

public sealed record CreateCommentInput(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid? ParentCommentId,
    string CaptchaKey,
    string CaptchaAnswer);
