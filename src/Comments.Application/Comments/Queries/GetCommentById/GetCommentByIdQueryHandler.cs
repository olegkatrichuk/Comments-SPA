using Comments.Application.DTOs;
using Comments.Application.Mapping;
using Comments.Domain.Interfaces;
using MediatR;

namespace Comments.Application.Comments.Queries.GetCommentById;

public sealed class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, CommentDto?>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentByIdQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentDto?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdWithRepliesAsync(request.Id, cancellationToken);

        return comment?.ToDto();
    }
}
