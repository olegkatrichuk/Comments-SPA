using Comments.Domain.Interfaces;
using Comments.Infrastructure.Messaging.Events;
using Comments.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Comments.Infrastructure.Messaging.Consumers;

public sealed class CommentCreatedConsumer : IConsumer<CommentCreatedIntegrationEvent>
{
    private readonly ICommentSearchService _searchService;
    private readonly ICommentNotificationService _notificationService;
    private readonly ILogger<CommentCreatedConsumer> _logger;

    public CommentCreatedConsumer(
        ICommentSearchService searchService,
        ICommentNotificationService notificationService,
        ILogger<CommentCreatedConsumer> logger)
    {
        _searchService = searchService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CommentCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Processing CommentCreatedIntegrationEvent for comment {CommentId}",
            message.CommentId);

        try
        {
            await _searchService.IndexAsync(
                message.CommentId,
                message.UserName,
                message.Email,
                message.Text,
                message.CreatedAt,
                context.CancellationToken);

            _logger.LogInformation(
                "Indexed comment {CommentId} in Elasticsearch",
                message.CommentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to index comment {CommentId} in Elasticsearch",
                message.CommentId);
        }

        try
        {
            await _notificationService.NotifyCommentCreated(message);

            _logger.LogInformation(
                "Notified clients about comment {CommentId} via SignalR",
                message.CommentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to notify clients about comment {CommentId} via SignalR",
                message.CommentId);
        }
    }
}
