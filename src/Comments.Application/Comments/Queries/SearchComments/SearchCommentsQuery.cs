using Comments.Application.DTOs;
using MediatR;

namespace Comments.Application.Comments.Queries.SearchComments;

public sealed record SearchCommentsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 25) : IRequest<PagedResult<CommentDto>>;
