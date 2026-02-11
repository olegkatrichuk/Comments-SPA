using Comments.Domain.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;

namespace Comments.Infrastructure.Search;

public sealed class ElasticsearchCommentSearchService : ICommentSearchService
{
    private const string IndexName = "comments";
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchCommentSearchService> _logger;
    private volatile bool _indexEnsured;

    public ElasticsearchCommentSearchService(
        ElasticsearchClient client,
        ILogger<ElasticsearchCommentSearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task IndexAsync(
        Guid commentId,
        string userName,
        string email,
        string text,
        DateTime createdAt,
        CancellationToken ct = default)
    {
        await EnsureIndexExistsAsync(ct);

        var document = new CommentSearchDocument
        {
            Id = commentId,
            UserName = userName,
            Email = email,
            Text = text,
            CreatedAt = createdAt
        };

        var response = await _client.IndexAsync(document, idx => idx
            .Index(IndexName)
            .Id(commentId.ToString()),
            ct);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index comment {CommentId} in Elasticsearch: {Error}",
                commentId,
                response.DebugInformation);
        }
    }

    public async Task<(IReadOnlyList<Guid> CommentIds, int TotalCount)> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        await EnsureIndexExistsAsync(ct);

        var response = await _client.SearchAsync<CommentSearchDocument>(s => s
            .Index(IndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(query)
                    .Fields(new[] { "userName", "email", "text" })
                    .Fuzziness(new Fuzziness("AUTO"))
                )
            ),
            ct);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to search comments in Elasticsearch: {Error}",
                response.DebugInformation);

            return (Array.Empty<Guid>(), 0);
        }

        var commentIds = response.Documents
            .Select(d => d.Id)
            .ToList()
            .AsReadOnly();

        var totalCount = (int)response.Total;

        return (commentIds, totalCount);
    }

    private async Task EnsureIndexExistsAsync(CancellationToken ct)
    {
        if (_indexEnsured)
            return;

        var existsResponse = await _client.Indices.ExistsAsync(IndexName, ct);

        if (!existsResponse.Exists)
        {
            var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Mappings(m => m
                    .Properties<CommentSearchDocument>(p => p
                        .Keyword(k => k.Id)
                        .Text(t => t.UserName)
                        .Text(t => t.Email)
                        .Text(t => t.Text)
                        .Date(d => d.CreatedAt)
                    )
                ),
                ct);

            if (!createResponse.IsValidResponse)
            {
                _logger.LogError(
                    "Failed to create Elasticsearch index '{IndexName}': {Error}",
                    IndexName,
                    createResponse.DebugInformation);

                return;
            }

            _logger.LogInformation("Created Elasticsearch index '{IndexName}'", IndexName);
        }

        _indexEnsured = true;
    }
}
