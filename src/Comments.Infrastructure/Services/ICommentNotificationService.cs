using Comments.Infrastructure.Messaging.Events;

namespace Comments.Infrastructure.Services;

public interface ICommentNotificationService
{
    Task NotifyCommentCreated(CommentCreatedIntegrationEvent evt);
}
