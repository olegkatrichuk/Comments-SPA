using Comments.Infrastructure.Hubs;
using Comments.Infrastructure.Messaging.Events;
using Microsoft.AspNetCore.SignalR;

namespace Comments.Infrastructure.Services;

public sealed class CommentNotificationService : ICommentNotificationService
{
    private readonly IHubContext<CommentsHub> _hubContext;

    public CommentNotificationService(IHubContext<CommentsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyCommentCreated(CommentCreatedIntegrationEvent evt)
    {
        await _hubContext.Clients.All.SendAsync("CommentCreated", new
        {
            evt.CommentId,
            evt.UserName,
            evt.Email,
            evt.Text,
            evt.CreatedAt
        });
    }
}
