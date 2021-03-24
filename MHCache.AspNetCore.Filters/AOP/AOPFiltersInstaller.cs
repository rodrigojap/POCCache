using System;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using MHCache.AspNetCore.Filters.MVC.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.MVC
{
    public static class AOPFiltersInstaller
    {
        public static IServiceCollection InstallRedisAOPCacheFilter(this IServiceCollection services, params Type[] types)
        {
            services
                .ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<CachedAOPAttribute>(
                                                                        types.Select(o => Predicates.Implement(o)).ToArray()
                                                                    );
                    
                });

            return services;
        }

        public static IServiceCollection InstallRedisAOPCacheFilter(this IServiceCollection services)
        {
            services
                .ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<CachedAOPAttribute>();
                });

            return services;
        }
    }
}
