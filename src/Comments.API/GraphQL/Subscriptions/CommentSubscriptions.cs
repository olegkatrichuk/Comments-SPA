using Comments.Application.DTOs;

namespace Comments.API.GraphQL.Subscriptions;

[SubscriptionType]
public sealed class CommentSubscriptions
{
    [Subscribe]
    [Topic]
    public CommentDto OnCommentCreated([EventMessage] CommentDto comment) => comment;
}
