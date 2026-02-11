using Comments.Application.DTOs;
using Comments.Domain.Enums;
using MediatR;

namespace Comments.Application.Comments.Queries.GetComments;

public sealed record GetCommentsQuery(
    int Page = 1,
    int PageSize = 25,
    SortField SortField = SortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending) : IRequest<PagedResult<CommentDto>>;
