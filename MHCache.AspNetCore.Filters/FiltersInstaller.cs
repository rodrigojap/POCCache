using MHCache.AspNetCore.Filters.DataModel;
using MHCache.Installation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters
{
    public static class FiltersInstaller
    {
        public static IServiceCollection InstallMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration)
            => services
                .InstallMHRedisCache(configuration)
                .GetConfigurationDataMHRedisCacheFilters(configuration);
        
        public static MvcOptions InstallMHRedisCacheFilter(this MvcOptions options)
        {
            options.Filters.Add<CachedByConfigurationAttribute>();
            return options;
        }

        public static MvcOptions InstallMHRedisCacheRemoveFilter(this MvcOptions options)
        {
            options.Filters.Add<RemoveCacheByConfigurationAttribute>();
            return options;
        }

        private static IServiceCollection GetConfigurationDataMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration) 
        {
            services
                .AddOptions<FilterCacheConfiguration>()
                .Configure(options => {
                    configuration.GetSection("MHCache:FilterCacheConfiguration").Bind(options);
                });

            return services;
        }
    }
}
