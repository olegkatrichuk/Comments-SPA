namespace Comments.Application.DTOs;

public sealed record AttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    string Url);
