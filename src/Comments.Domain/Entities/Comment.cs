using Comments.Domain.Events;
using Comments.Domain.ValueObjects;

namespace Comments.Domain.Entities;

public sealed class Comment
{
    private readonly List<Comment> _replies = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; private set; }
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? HomePage { get; private set; }
    public string Text { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public Guid? ParentCommentId { get; private set; }
    public Comment? ParentComment { get; private set; }
    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();
    public Attachment? Attachment { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Comment() { }

    public static Comment Create(
        ValueObjects.UserName userName,
        ValueObjects.Email email,
        HomePage? homePage,
        string text,
        Guid? parentCommentId = null)
    {
        var comment = new Comment
        {
            Id = Guid.CreateVersion7(),
            UserName = userName.Value,
            Email = email.Value,
            HomePage = homePage?.Value,
            Text = text,
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = parentCommentId
        };

        comment._domainEvents.Add(new CommentCreatedEvent(comment.Id));
        return comment;
    }

    public void SetAttachment(Attachment attachment)
    {
        Attachment = attachment;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
