using System.Threading.Tasks;

namespace CacheStore.Redis
{
    public interface IRedisStore
    {
        Task SetCacheData(string key, object obj, int absoluteExpiration = 0, int slidingExpiration = 0);
        Task<byte[]> GetCacheData(string key);
    }
}
