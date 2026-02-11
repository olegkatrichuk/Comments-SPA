using System.Text.Json;
using Comments.Domain.Interfaces;
using StackExchange.Redis;

namespace Comments.Infrastructure.Cache;

public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        var db = _connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<T>(value!, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        where T : class
    {
        var db = _connectionMultiplexer.GetDatabase();
        var serialized = JsonSerializer.Serialize(value, JsonOptions);
        if (expiration.HasValue)
            await db.StringSetAsync(key, serialized, new Expiration(expiration.Value));
        else
            await db.StringSetAsync(key, serialized);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        var db = _connectionMultiplexer.GetDatabase();

        foreach (var endpoint in endpoints)
        {
            var server = _connectionMultiplexer.GetServer(endpoint);

            if (server.IsReplica)
                continue;

            var keys = server.Keys(
                database: db.Database,
                pattern: $"{prefix}*",
                pageSize: 1000);

            var keyArray = keys.ToArray();

            if (keyArray.Length > 0)
            {
                await db.KeyDeleteAsync(keyArray);
            }
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
