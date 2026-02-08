using Comments.Application.Comments.Queries.GetCommentById;
using Comments.Application.Comments.Queries.GetComments;
using Comments.Application.Comments.Queries.SearchComments;
using Comments.Application.DTOs;
using Comments.Domain.Enums;
using MediatR;

namespace Comments.API.GraphQL.Queries;

[QueryType]
public sealed class CommentQueries
{
    public async Task<PagedResult<CommentDto>> GetComments(
        int page,
        int pageSize,
        SortField sortField,
        SortDirection sortDirection,
        [Service] IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCommentsQuery(page, pageSize, sortField, sortDirection);
        return await mediator.Send(query, ct);
    }

    public async Task<CommentDto?> GetCommentById(
        Guid id,
        [Service] IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCommentByIdQuery(id);
        return await mediator.Send(query, ct);
    }

    public async Task<PagedResult<CommentDto>> SearchComments(
        string term,
        int page,
        int pageSize,
        [Service] IMediator mediator,
        CancellationToken ct)
    {
        var query = new SearchCommentsQuery(term, page, pageSize);
        return await mediator.Send(query, ct);
    }
}
