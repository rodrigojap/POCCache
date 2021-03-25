using System;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.AspNetCore.Filters.MVC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.AOP
{
    public static class AOPFiltersInstaller
    {
        public static IServiceCollection InstallRedisAOPCacheFilter(
                                                                       this IServiceCollection services, 
                                                                       IConfiguration configuration, 
                                                                       params Type[] types
                                                                   )
        {
            services
                .InstallMHRedisCache(configuration)
                .GetConfigurationDataMHRedisCacheFilters(configuration)
                .ConfigureDynamicProxy(config =>
                {
                    config
                        .Interceptors
                        .AddTyped<CachedAOPByConfiguration>(
                                                               types.Select(o => Predicates.Implement(o)).ToArray()
                                                           )
                        .AddTyped<CachedRemoveAOPByConfiguration>(
                                                                     types.Select(o => Predicates.Implement(o)).ToArray()
                                                                 );
                    
                });

            return services;
        }        

        public static IServiceCollection InstallRedisAOPCacheFilter(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .InstallMHRedisCache(configuration)
                .GetConfigurationDataMHRedisCacheFilters(configuration)
                .ConfigureDynamicProxy(config =>
                {
                    config
                        .Interceptors
                        .AddTyped<CachedAOPByConfiguration>()
                        .AddTyped<CachedRemoveAOPByConfiguration>();
                });

            return services;
        }

        private static IServiceCollection GetConfigurationDataMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<FilterCachedConfiguration>()
                .Configure(options => {
                    configuration.GetSection("MHCache:FilterCacheConfiguration").Bind(options);
                });

            return services;
        }
    }
}
