using System;
using System.Threading.Tasks;

namespace MHCache.Services
{
    public interface IResponseCacheService
    {
        Task<string> GetAllKeys();

        Task<bool> ContainsKey(string key);

        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive);

        Task<string> GetCachedResponseAsStringAsync(string cacheKey);

        Task RemoveCachedResponseAsync(string cacheKey);
    }
}
