using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace MHCache.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            //Microsoft Default Cache Object
            _distributedCache = distributedCache;
            
            //Redis Object (full methods)
            _connectionMultiplexer = connectionMultiplexer;
        }        

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive)
        {
            if (response == null)
            {
                return;
            }

            var serializedResponse = JsonSerializer.Serialize(response);


            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeTimeLive
            });
        }
       
        public async Task<string> GetCachedResponseAsStringAsync(string cacheKey)
        {            
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);                          
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }

        public Task RemoveCachedResponseAsync(string cacheKey) 
        {
            return _distributedCache.RemoveAsync(cacheKey);
        }

        public Task<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
