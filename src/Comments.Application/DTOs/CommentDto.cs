namespace Comments.Application.DTOs;

public sealed record CommentDto(
    Guid Id,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    DateTime CreatedAt,
    AttachmentDto? Attachment,
    List<CommentDto> Replies);
