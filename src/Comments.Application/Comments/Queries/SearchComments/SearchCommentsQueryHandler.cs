using Comments.Application.DTOs;
using Comments.Application.Mapping;
using Comments.Domain.Interfaces;
using MediatR;

namespace Comments.Application.Comments.Queries.SearchComments;

public sealed class SearchCommentsQueryHandler : IRequestHandler<SearchCommentsQuery, PagedResult<CommentDto>>
{
    private readonly ICommentSearchService _searchService;
    private readonly ICommentRepository _commentRepository;

    public SearchCommentsQueryHandler(
        ICommentSearchService searchService,
        ICommentRepository commentRepository)
    {
        _searchService = searchService;
        _commentRepository = commentRepository;
    }

    public async Task<PagedResult<CommentDto>> Handle(SearchCommentsQuery request, CancellationToken cancellationToken)
    {
        var (commentIds, totalCount) = await _searchService.SearchAsync(
            request.SearchTerm, request.Page, request.PageSize, cancellationToken);

        var dtos = new List<CommentDto>(commentIds.Count);

        foreach (var id in commentIds)
        {
            var comment = await _commentRepository.GetByIdWithRepliesAsync(id, cancellationToken);
            if (comment is not null)
                dtos.Add(comment.ToDto());
        }

        return new PagedResult<CommentDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
