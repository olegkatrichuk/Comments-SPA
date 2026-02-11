using Comments.Application.DTOs;
using MediatR;

namespace Comments.Application.Comments.Commands.CreateComment;

public sealed record CreateCommentCommand(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid? ParentCommentId,
    string CaptchaKey,
    string CaptchaAnswer,
    Stream? FileStream,
    string? FileName,
    string? FileContentType) : IRequest<CommentDto>;
