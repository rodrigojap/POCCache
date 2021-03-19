using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MHCache.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        private readonly IServer _server;

        public ResponseCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            //Database 
            _database = connectionMultiplexer.GetDatabase();

            //Server 
            var endpoint = connectionMultiplexer.GetEndPoints()[0];
            _server = connectionMultiplexer.GetServer(endpoint);
        }        

        public Task<bool> SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive)
        {
            if (response == null)
            {
                return Task.FromResult(false);
            }

            var serializedResponse = JsonSerializer.Serialize(response);
            
            return _database.StringSetAsync(
                                              new RedisKey(cacheKey), 
                                              serializedResponse,
                                              timeTimeLive
                                          );
        }
       
        public async Task<string> GetCachedResponseAsStringAsync(string cacheKey)
        {            
            string cachedResponse = await _database.StringGetAsync(
                                              new RedisKey(cacheKey)
                                          );
            
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }

        public Task<bool> RemoveCachedResponseAsync(string cacheKey) 
        {
            return _database.KeyDeleteAsync(new RedisKey(cacheKey));
        }

        public IEnumerable<string> GetKeysByPattern(string pattern, int pageSize = 250, int pageOffset = 0)
        {
            var keys = _server.Keys(pattern : new RedisValue(pattern), pageSize : pageSize, pageOffset: pageOffset);
            var keyList = _server.Keys().ToArray();
            return keyList.Select(key => key.ToString()).ToArray();
        }

        public Task<bool> ContainsKey(string cacheKey)
        {
            return _database.KeyExistsAsync(new RedisKey(cacheKey));
        }
    }
}
