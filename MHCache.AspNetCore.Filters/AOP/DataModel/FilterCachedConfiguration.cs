using System;
using System.Collections.Generic;
using System.Linq;

namespace MHCache.AspNetCore.Filters.AOP.DataModel
{
    /// <summary>Dados de Configuração de cache no filtro</summary>
    public class FilterCachedConfiguration
    {
        /// <summary>Tempo de expiração da cache geral</summary>
        public int? GeneralTimeToLiveSeconds { get; set; }

        public string[] RegisterCachedByOnlyTypeNames { get; set; }

        public IEnumerable<Type> RegisterCachedByOnlyTypes { get => RegisterCachedByOnlyTypeNames?.Select(o => Type.GetType(o, true, true)); }

        public string[] RegisterCachedRemoveByOnlyTypeNames { get; set; }

        public IEnumerable<Type> RegisterCachedRemoveByOnlyTypes { get => RegisterCachedRemoveByOnlyTypeNames?.Select(o => Type.GetType(o, true, true)); }

        /// <summary>Nome da controllers que serão executadas em cache</summary>
        public IEnumerable<MethodCachedConfiguration> CachedMethods { get; set; }
        
        /// <summary>Nome de controllers que quando chamadas em Post/Put/Delete removerá o cache</summary>
        public IEnumerable<MethodCachedRemoveConfiguration> CachedRemoveMethods { get; set; }
    }
}
