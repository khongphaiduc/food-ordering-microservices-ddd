namespace search_service.SearchService.Infrastructure.Redis.Interface
{
    public interface IRedisLockService
    {
        Task<bool> AcquireAsync(string key, string value, TimeSpan expiry);
        Task ReleaseAsync(string key, string value);

    }
}
