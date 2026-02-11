namespace Comments.Domain.Interfaces;

public interface ICommentSearchService
{
    Task IndexAsync(Guid commentId, string userName, string email, string text, DateTime createdAt, CancellationToken ct = default);
    Task<(IReadOnlyList<Guid> CommentIds, int TotalCount)> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default);
}
