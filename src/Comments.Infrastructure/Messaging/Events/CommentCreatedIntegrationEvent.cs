namespace Comments.Infrastructure.Messaging.Events;

public sealed record CommentCreatedIntegrationEvent(
    Guid CommentId,
    string UserName,
    string Email,
    string Text,
    DateTime CreatedAt);
