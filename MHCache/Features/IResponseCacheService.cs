using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MHCache.Features
{
    /// <summary>Interface para tratamento de cache no serviço</summary>
    public interface IResponseCacheService
    {
        /// <summary>Indica se a chave indicada existe no redis</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        Task<bool> ContainsKeyAsync(string cacheKey);

        /// <summary>Seta um texto em cache no serviço e retorna caso realizado com sucesso</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        /// <param name="value">Valor em texto a ser cacheado</param>
        /// <param name="timeLive">Tempo de expiração da cache</param>
        Task<bool> SetCacheResponseAsync(string cacheKey, string value, TimeSpan? timeLive);

        /// <summary>Obtém a informação de uma cache como texto a partir da chave</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        Task<string> GetCachedResponseAsStringAsync(string cacheKey);

        /// <summary>Obtém todas as chaves a partir de um padrão indicado</summary>
        /// <param name="pattern">Texto de padrão para busca das chaves (Use * para ignorar prefixo ou sufixo)</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="pageOffset">Indice da página</param>
        IEnumerable<string> GetKeysByPattern(string pattern, int pageSize = 250, int pageOffset = 0);

        /// <summary>Remove itens a partir das chaves indicadas e retorna o numero de itens removidos</summary>
        /// <param name="cacheKey">Nome da chave de cache</param>
        Task<long> RemoveCachedResponseByNamesAsync(params string[] cacheKeys);

    }
}
