using System;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters
{
    public static class AOPFiltersInstaller
    {
        public static IServiceCollection InstallRedisAOPCacheFilter(this IServiceCollection services, params Type[] types)
        {
            // Add Interceptors so Dynamic Proxy can do its work and intercept
            //services.ConfigureDynamicProxy(config => config.Interceptors.AddTyped<CachedAOPAttribute>());
            // Normally you build normally(with .Build). 
            //var serviceProvider = services.BuildDynamicProxyProvider();

            services
                .ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<CachedAOPAttribute>(
                                                                        types.Select(o => Predicates.Implement(o)).ToArray()
                                                                    );
                    config.ThrowAspectException = true;
                    
                });

            //services.BuildAspectInjectorProvider();

            return services;
        }
    }
}
