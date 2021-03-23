using System;
using System.Threading.Tasks;
using MHCache.Services;

namespace MHCache.Extensions
{
    public static class ResponseCacheMethodExtensions
    {
        public static async Task<T> CacheResponseMethodAsync<T>(
                                                                    this IResponseCacheService cacheService, 
                                                                    string keyName, 
                                                                    TimeSpan? timeSpan, 
                                                                    Func<T> func
                                
                                                               ) 
        {
            if (await cacheService.ContainsKey(keyName))
            {
                return await cacheService.GetCachedResponseAsync<T>(keyName);
            }

            var result = func.Invoke();

            await cacheService.SetCacheResponseAsync(keyName, result, timeSpan);

            return result;
        }     
    }
}
