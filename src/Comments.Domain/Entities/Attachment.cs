using Comments.Domain.Enums;

namespace Comments.Domain.Entities;

public sealed class Attachment
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = default!;
    public string StoredFileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long FileSizeBytes { get; private set; }
    public AttachmentType Type { get; private set; }
    public Guid CommentId { get; private set; }
    public Comment Comment { get; private set; } = default!;

    private Attachment() { }

    public static Attachment Create(
        string fileName,
        string storedFileName,
        string contentType,
        long fileSizeBytes,
        AttachmentType type)
    {
        return new Attachment
        {
            Id = Guid.CreateVersion7(),
            FileName = fileName,
            StoredFileName = storedFileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            Type = type
        };
    }
}
