using Comments.API.GraphQL.Inputs;
using Comments.API.GraphQL.Subscriptions;
using Comments.Application.Comments.Commands.CreateComment;
using Comments.Application.DTOs;
using HotChocolate.Subscriptions;
using MediatR;

namespace Comments.API.GraphQL.Mutations;

[MutationType]
public sealed class CommentMutations
{
    public async Task<CommentDto> CreateComment(
        CreateCommentInput input,
        [Service] IMediator mediator,
        [Service] ITopicEventSender eventSender,
        CancellationToken ct)
    {
        var command = new CreateCommentCommand(
            input.UserName,
            input.Email,
            input.HomePage,
            input.Text,
            input.ParentCommentId,
            input.CaptchaKey,
            input.CaptchaAnswer,
            FileStream: null,
            FileName: null,
            FileContentType: null);

        var result = await mediator.Send(command, ct);

        await eventSender.SendAsync(nameof(CommentSubscriptions.OnCommentCreated), result, ct);

        return result;
    }
}
