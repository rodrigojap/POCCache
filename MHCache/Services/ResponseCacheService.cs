using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

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

            var serializedResponse = JsonConvert.SerializeObject(response);
            
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
    }

    public static class ResponseCacheServiceExtensions 
    {
        public static Task CacheResponseAsync<T>(this IResponseCacheService cacheService, T response, TimeSpan timeTimeLive)
        {
            return cacheService.SetCacheResponseAsync(typeof(T).FullName, response, timeTimeLive);
        }

        public static Task<T> GetCachedResponseAsync<T>(this IResponseCacheService cacheService, string cacheKey)
        {
            return cacheService
                    .GetCachedResponseAsStringAsync(cacheKey)
                    .ContinueWith(o =>  JsonConvert.DeserializeObject<T>( o.Result ));
        }

        public static Task<T> GetCachedResponseAsync<T>(this IResponseCacheService cacheService)
        {
            return cacheService
                   .GetCachedResponseAsStringAsync(typeof(T).FullName)
                   .ContinueWith(o => JsonConvert.DeserializeObject<T>(o.Result));
        }

    }
}
