using Comments.Domain.Events;
using Comments.Domain.Interfaces;
using Comments.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace Comments.Infrastructure.Messaging.Handlers;

public sealed class CommentCreatedEventHandler : INotificationHandler<CommentCreatedEvent>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CommentCreatedEventHandler(
        ICommentRepository commentRepository,
        IPublishEndpoint publishEndpoint)
    {
        _commentRepository = commentRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(notification.CommentId, cancellationToken);

        if (comment is null)
            return;

        await _publishEndpoint.Publish(new CommentCreatedIntegrationEvent(
            comment.Id,
            comment.UserName,
            comment.Email,
            comment.Text,
            comment.CreatedAt), cancellationToken);
    }
}
