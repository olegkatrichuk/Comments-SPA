namespace Comments.Domain.Events;

public sealed record CommentCreatedEvent(Guid CommentId) : IDomainEvent;
