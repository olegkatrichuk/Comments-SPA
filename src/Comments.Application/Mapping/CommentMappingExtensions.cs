using Comments.Application.DTOs;
using Comments.Domain.Entities;

namespace Comments.Application.Mapping;

public static class CommentMappingExtensions
{
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto(
            Id: comment.Id,
            UserName: comment.UserName,
            Email: comment.Email,
            HomePage: comment.HomePage,
            Text: comment.Text,
            CreatedAt: comment.CreatedAt,
            Attachment: comment.Attachment?.ToDto(),
            Replies: comment.Replies
                .OrderBy(r => r.CreatedAt)
                .Select(r => r.ToDto())
                .ToList());
    }

    public static AttachmentDto ToDto(this Attachment attachment)
    {
        return new AttachmentDto(
            Id: attachment.Id,
            FileName: attachment.FileName,
            ContentType: attachment.ContentType,
            Url: $"/api/files/{attachment.StoredFileName}");
    }

    public static List<CommentDto> ToDtoList(this IEnumerable<Comment> comments)
    {
        return comments.Select(c => c.ToDto()).ToList();
    }
}
