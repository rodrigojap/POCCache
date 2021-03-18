using System;
using System.Threading.Tasks;

namespace MHCache.Services
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive);

        Task<string> GetCachedResponseAsStringAsync(string cacheKey);
    }
}
