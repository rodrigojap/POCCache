using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MHCache.Features
{
    /// <summary>Classe de tratamento de cache no serviço</summary>
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        private readonly IServer _server;

        public ResponseCacheService(IConnectionMultiplexer connectionMultiplexer)
        { 
            _database = connectionMultiplexer.GetDatabase();

            var endpoint = connectionMultiplexer.GetEndPoints().FirstOrDefault();
            _server = connectionMultiplexer.GetServer(endpoint);
        }

        /// <summary>Indica se a chave indicada existe no redis</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        public Task<bool> ContainsKeyAsync(string cacheKey)
        {
            return _database.KeyExistsAsync(new RedisKey(cacheKey));
        }

        /// <summary>Seta um texto em cache no serviço e retorna caso realizado com sucesso</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        /// <param name="value">Valor em texto a ser cacheado</param>
        /// <param name="timeLive">Tempo de expiração da cache</param>
        public Task<bool> SetCacheResponseAsync(string cacheKey, string value, TimeSpan? timeLive)
        {
            if (string.IsNullOrWhiteSpace(value)) 
            {
                return Task.FromResult(false);
            }

            if (string.IsNullOrWhiteSpace(cacheKey)) 
            {
                throw new Exception("Não pode ser criado um item com chave nula o string vazia.");
            }

            return _database.StringSetAsync(
                                              new RedisKey(cacheKey),
                                              value,
                                              timeLive
                                          );
        }

        /// <summary>Obtém a informação de uma cache como texto a partir da chave</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        public async Task<string> GetCachedResponseAsStringAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return null;
            }

            string cachedResponse = await _database.StringGetAsync(
                                              new RedisKey(cacheKey)
                                          );
            
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }

        /// <summary>Obtém todas as chaves a partir de um padrão indicado</summary>
        /// <param name="pattern">Texto de padrão para busca das chaves (Use * para ignorar prefixo ou sufixo)</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="pageOffset">Indice da página</param>
        public IEnumerable<string> GetKeysByPattern(string pattern, int pageSize = 250, int pageOffset = 0)
        {
            var keys = _server
                         .Keys(pattern: new RedisValue(pattern), pageSize: pageSize, pageOffset: pageOffset)
                         .ToArray();
            return keys.Select(key => key.ToString()).ToArray();
        }

        /// <summary>Remove um item de cache a partir no nome indicado</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        public Task<long> RemoveCachedResponseByNamesAsync(params string[] cacheKeys) 
        {
            return _database.KeyDeleteAsync(cacheKeys.Select(o => new RedisKey(o)).ToArray());
        }
    }
}
