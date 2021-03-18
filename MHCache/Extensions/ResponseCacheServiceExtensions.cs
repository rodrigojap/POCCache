using System;
using System.Threading.Tasks;
using MHCache.Services;
using Newtonsoft.Json;

namespace MHCache.Extensions
{
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
