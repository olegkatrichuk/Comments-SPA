using Comments.Application.DTOs;
using Comments.Application.Mapping;
using Comments.Domain.Interfaces;
using MediatR;

namespace Comments.Application.Comments.Queries.GetComments;

public sealed class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly ICommentRepository _commentRepository;
    private readonly ICacheService _cacheService;

    public GetCommentsQueryHandler(
        ICommentRepository commentRepository,
        ICacheService cacheService)
    {
        _commentRepository = commentRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"comments:page:{request.Page}:size:{request.PageSize}:sort:{request.SortField}:{request.SortDirection}";

        // Try cache first
        var cached = await _cacheService.GetAsync<PagedResult<CommentDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        // Fall back to database
        var (items, totalCount) = await _commentRepository.GetTopLevelPagedAsync(
            request.Page,
            request.PageSize,
            request.SortField,
            request.SortDirection,
            cancellationToken);

        var dtos = items.ToDtoList();

        var result = new PagedResult<CommentDto>(dtos, totalCount, request.Page, request.PageSize);

        // Cache the result
        await _cacheService.SetAsync(cacheKey, result, CacheTtl, cancellationToken);

        return result;
    }
}
