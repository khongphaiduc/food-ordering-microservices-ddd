
using StackExchange.Redis;

namespace food_service.ProductService.Infrastructure.RedisService.RedisInterface
{
    public class RedisLockService : IRedisLockService
    {
        private readonly IDatabase _dbRedis;
        public RedisLockService(IConnectionMultiplexer connectionMultiplexer)
        {
            _dbRedis = connectionMultiplexer.GetDatabase(0);
        }

        public async Task<bool> AcquireAsync(string key, string value, TimeSpan expiry)
        {
            return await _dbRedis.StringSetAsync(
               key,
               value,
               expiry,
               When.NotExists
             );
        }

        public async Task ReleaseAsync(string key, string value)
        {
            const string lua = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
            return redis.call('del', KEYS[1])
        else
            return 0
        end";

            await _dbRedis.ScriptEvaluateAsync(
                lua,
                new RedisKey[] { key },
                new RedisValue[] { value }
            );
        }
    }
}
