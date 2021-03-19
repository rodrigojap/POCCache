using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MHCache.Services;

namespace MHCache.Extensions
{
    public static class ResponseCacheServiceExtensions 
    {
        public static Task CacheResponseAsync<T>(
                                                    this IResponseCacheService cacheService, 
                                                    T response, 
                                                    TimeSpan timeTimeLive
                                                )
            => cacheService
                    .SetCacheResponseAsync(typeof(T).FullName, response, timeTimeLive);

        public static Task<T> GetCachedResponseAsync<T>(
                                                            this IResponseCacheService cacheService, 
                                                            string cacheKey
                                                       )
            => cacheService
                    .GetCachedResponseAsStringAsync(cacheKey)
                    .ContinueWith(o => JsonSerializer.Deserialize<T>(o.Result));

        public static Task<T> GetCachedResponseAsync<T>(this IResponseCacheService cacheService)
             => cacheService
                   .GetCachedResponseAsStringAsync(typeof(T).FullName)
                   .ContinueWith(o => JsonSerializer.Deserialize<T>(o.Result));

        public static IEnumerable<string> GetAllKeys(
                                                        this IResponseCacheService cacheService,
                                                        int pageSize = 250,
                                                        int pageOffset = 0
                                                    )
            => cacheService.GetKeysByPattern("*", pageSize, pageOffset);

        public static Task RemoveAllByPattern(
                                                this IResponseCacheService cacheService,
                                                string pattern
                                             ) 
        {
            var keys = cacheService.GetKeysByPattern("*", int.MaxValue, 0);
            return Task
                    .WhenAll(
                        keys.Select(key => cacheService.RemoveCachedResponseAsync(key))
                    );                   
        }
    }
}
