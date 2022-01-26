using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace CacheStore.Redis
{
    public class RedisStore: IRedisStore
    {
        private readonly IDistributedCache distributedCache;
        public RedisStore(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<byte[]> GetCacheData(string key)
        {
            var redisCacheData = await distributedCache.GetAsync(key);
            return redisCacheData;
        }

        public async Task SetCacheData(string key, object obj, int absoluteExpiration = 0, int slidingExpiration = 0)
        {
            
            var serializedData = JsonConvert.SerializeObject(obj);
            var redisData = Encoding.UTF8.GetBytes(serializedData);
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiration != 0)
            {
                options.SetAbsoluteExpiration(DateTime.Now.AddMinutes(absoluteExpiration));
            }

            if (slidingExpiration != 0)
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpiration));
            }
            await distributedCache.SetAsync(key, redisData);
        }
    }
}
