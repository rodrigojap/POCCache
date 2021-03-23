using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MHCache.Services;

namespace MHCache.Extensions
{
    /// <summary>Métodos de extensão do serviço de cache</summary>
    public static class ResponseCacheServiceExtensions 
    {
        /// <summary>Seta um objeto do tipo TResult, onde o nome da chave é o seu Nome completo do tipo</summary>
        /// <typeparam name="TValue">O tipo do objeto a ser inserido</typeparam>
        /// <param name="cacheService">Serviço de cache</param>
        /// <param name="cacheKey">Nome da chave de cache</param>
        /// <param name="objectValue">O valor do objeto a ser inserido na cache</param>
        /// <param name="timeTimeLive">Tempo de expiração do objeto</param>
        public static Task SetCacheResponseAsync<TValue>(
                                                    this IResponseCacheService cacheService,
                                                    string cacheKey,
                                                    TValue objectValue,
                                                    TimeSpan? timeTimeLive
                                                )
        {
            if (objectValue == null)
            {
                return Task.FromResult(false);
            }

            return cacheService
                    .SetCacheResponseAsync(cacheKey, JsonSerializer.Serialize(objectValue), timeTimeLive);
        } 

        /// <summary>Seta um objeto do tipo TResult, onde o nome da chave é o seu Nome completo do tipo</summary>
        /// <typeparam name="TValue">O tipo do objeto a ser inserido</typeparam>
        /// <param name="cacheService">Serviço de cache</param>
        /// <param name="objectValue">O valor do objeto a ser inserido na cache</param>
        /// <param name="timeTimeLive">Tempo de expiração do objeto</param>
        public static Task SetCacheResponseAsync<TValue>(
                                                    this IResponseCacheService cacheService, 
                                                    TValue objectValue, 
                                                    TimeSpan? timeTimeLive
                                                )
            => cacheService
                    .SetCacheResponseAsync(typeof(TValue).FullName, objectValue, timeTimeLive);

        /// <summary>Obtém o valor de uma cache a partir da chave indicada</summary>
        /// <typeparam name="TValue">O tipo do objeto a ser inserido</typeparam>
        /// <param name="cacheService">Serviço de cache</param>
        /// <param name="cacheKey">Nome da chave de cache</param>
        public static Task<TValue> GetCachedResponseAsync<TValue>(
                                                                     this IResponseCacheService cacheService, 
                                                                     string cacheKey
                                                                 )
            => cacheService
                    .GetCachedResponseAsStringAsync(cacheKey)
                    .ContinueWith(o => JsonSerializer.Deserialize<TValue>(o.Result));

        /// <summary>Obtém o valor de uma cache a partir da chave indicada</summary>
        /// <typeparam name="TValue">O tipo do objeto a ser inserido</typeparam>
        /// <param name="cacheService">Serviço de cache</param>
        public static Task<T> GetCachedResponseAsync<T>(this IResponseCacheService cacheService)
             => cacheService
                   .GetCachedResponseAsStringAsync(typeof(T).FullName)
                   .ContinueWith(o => JsonSerializer.Deserialize<T>(o.Result));

        /// <summary>Obtém todas a chaves do redis paginadas</summary>
        /// <param name="cacheService">Serviço de cache</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="pageOffset">Indice da página</param>
        public static IEnumerable<string> GetAllKeys(
                                                        this IResponseCacheService cacheService,
                                                        int pageSize = 250,
                                                        int pageOffset = 0
                                                    )
            => cacheService.GetKeysByPattern("*", pageSize, pageOffset);

        /// <summary>Remove todas as chaves a partir de um padrão</summary>
        /// <param name="cacheService">Serviço de cache</param>
        /// <param name="pattern">Texto de padrão para busca das chaves (Use * para ignorar prefixo ou sufixo)</param>
        public static Task RemoveAllByPattern(
                                                this IResponseCacheService cacheService,
                                                string pattern = "*"
                                             ) 
        {
            var keys = cacheService.GetKeysByPattern(pattern, int.MaxValue, 0);
            
            return Task
                    .WhenAll(
                        keys.Select(key => cacheService.RemoveCachedResponseAsync(key))
                    );                   
        }        
        
        /// <summary>
        /// Remove todo o cache do usuário
        /// </summary>
        /// <param name="cacheService"></param>
        /// <param name="userPattern">Deve ser o identificador único do usuário, será adicionado ':'</param>
        /// <returns></returns>
        public static Task RemoveAllByUserPattern(this IResponseCacheService cacheService,
                                                  string userPattern)
        {
            return cacheService.RemoveAllByPattern($"{userPattern}:*");
        }
    }
}
