using Comments.Application.DTOs;
using MediatR;

namespace Comments.Application.Comments.Queries.GetCommentById;

public sealed record GetCommentByIdQuery(Guid Id) : IRequest<CommentDto?>;
