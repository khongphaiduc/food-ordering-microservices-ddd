using search_service.SearchService.Infrastructure.Redis.Interface;
using StackExchange.Redis;

namespace search_service.SearchService.Infrastructure.Redis.Service
{
    public class RedisLockService : IRedisLockService
    {
        private IDatabase _databaseRedis;
        public RedisLockService(IConnectionMultiplexer connectionMultiplexer)
        {
            _databaseRedis = connectionMultiplexer.GetDatabase(1);
        }

        public async Task<bool> AcquireAsync(string key, string value, TimeSpan expiry)
        {
            return await _databaseRedis.StringSetAsync(key, value, expiry, When.NotExists);
        }

        public async Task ReleaseAsync(string key, string value)
        {
            const string lua = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
            return redis.call('del', KEYS[1])
        else
            return 0
        end";

            await _databaseRedis.ScriptEvaluateAsync(
                lua,
                new RedisKey[] { key },
                new RedisValue[] { value }
            );
        }
    }
}
