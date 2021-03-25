using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.AOP
{
    /// <summary>Realiza a instalação dos filtros no pipeline MVC</summary>
    public static class MVCFiltersInstaller
    {
        public static IServiceCollection InstallMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration)
            => services
                    .GetConfigurationDataMHRedisCacheFilters(configuration)
                    .InstallMHRedisCache(configuration);

        private static IServiceCollection GetConfigurationDataMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<FilterCachedConfiguration>()
                .Configure(options => {
                    configuration.GetSection("MHCache:FilterCacheConfiguration").Bind(options);
                });

            return services;
        }

        /// <summary>Instala o filtro pra realizar cache</summary>
        /// <param name="options">Opções do mvc para setar os filtros</param>
        public static MvcOptions InstallMHRedisCacheFilter(this MvcOptions options)
        {
            options.Filters.Add<CachedByConfigurationAttribute>();
            return options;
        }

        /// <summary>Realiza a instalação do filtro para remover o cache</summary>
        /// <param name="options">Opções do mvc para setar os filtros</param>
        public static MvcOptions InstallMHRedisCacheRemoveFilter(this MvcOptions options)
        {
            options.Filters.Add<CachedRemoveByConfigurationAttribute>();
            return options;
        }
    }
}
