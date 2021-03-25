using System.Collections.Generic;

namespace MHCache.AspNetCore.Filters.MVC.DataModel
{
    /// <summary>Dados de Configuração de cache no filtro</summary>
    public class FilterCachedConfiguration
    {
        /// <summary>Tempo de expiração da cache geral</summary>
        public int? GeneralTimeToLiveSeconds { get; set; }

        /// <summary>Nome da controllers que serão executadas em cache</summary>
        public IEnumerable<RouteCachedConfiguration> CachedRoutes { get; set; }
        
        /// <summary>Nome de controllers que quando chamadas em Post/Put/Delete removerá o cache</summary>
        public IEnumerable<RouteCachedRemoveConfiguration> CacheRemoveRoutes { get; set; }
    }
}
