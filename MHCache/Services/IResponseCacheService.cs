using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MHCache.Services
{
    public interface IResponseCacheService
    {
        IEnumerable<string> GetKeysByPattern(string pattern, int pageSize = 250, int pageOffset = 0);

        Task<bool> ContainsKey(string cacheKey);

        Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive);

        Task<string> GetCachedResponseAsStringAsync(string cacheKey);

        Task<bool> RemoveCachedResponseAsync(string cacheKey);
    }
}
