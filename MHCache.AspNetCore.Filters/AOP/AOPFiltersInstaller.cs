using System;
using System.Collections.Generic;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.AOP
{
    public static class AOPFiltersInstaller
    {
        public static IServiceCollection InstallRedisAOPCacheFilter(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new FilterCachedConfiguration();
            configuration.GetSection("MHCache:FilterCacheConfiguration").Bind(options);
            services
                .InstallMHRedisCache(configuration)
                .GetConfigurationDataMHRedisCacheFilters(configuration)
                .ConfigureDynamicProxy(config =>
                {
                    if (options.RegisterCachedByOnlyTypeNames != null && options.RegisterCachedByOnlyTypeNames.Any())
                    {
                        config
                            .Interceptors
                            .AddTyped<CachedAOPByConfiguration>(options.RegisterCachedByOnlyTypes.Select(o => Predicates.Implement(o)).ToArray());
                    }
                    else
                    {
                        config
                            .Interceptors
                            .AddTyped<CachedAOPByConfiguration>();
                    }

                    if (options.RegisterCachedRemoveByOnlyTypeNames != null && options.RegisterCachedRemoveByOnlyTypeNames.Any())
                    {
                        config
                            .Interceptors
                            .AddTyped<CachedRemoveAOPByConfiguration>(options.RegisterCachedRemoveByOnlyTypes.Select(o => Predicates.Implement(o)).ToArray());
                    }
                    else
                    {
                        config
                            .Interceptors
                            .AddTyped<CachedRemoveAOPByConfiguration>();
                    }    
                        
                });

            return services;
        }

        private static IServiceCollection GetConfigurationDataMHRedisCacheFilters(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<FilterCachedConfiguration>()
                .Configure(options => {
                    configuration.GetSection("MHCache:FilterCacheConfiguration").Bind(options);
                })
                .Validate( dataConfig => 
                {
                    var result = true;

                    if (
                          (dataConfig.RegisterCachedByOnlyTypeNames?.Any()).GetValueOrDefault() && 
                          (dataConfig.CachedMethods?.Any()).GetValueOrDefault()
                       ) 
                    {
                        var cachedMethodNames = dataConfig.RegisterCachedByOnlyTypes.GetMethodNames();
                        result = result && dataConfig
                                    .CachedMethods
                                    .All(patternMethodName => 
                                                cachedMethodNames
                                                .Any(methodName => methodName.Contains(patternMethodName.CachedMethodName))
                                    );
                    }

                    if (
                          (dataConfig.RegisterCachedRemoveByOnlyTypeNames?.Any()).GetValueOrDefault() &&
                          (dataConfig.CachedRemoveMethods?.Any()).GetValueOrDefault()
                       )
                    {
                        var cachedRemovedMethodNames = dataConfig.RegisterCachedByOnlyTypes.GetMethodNames();
                        result = result && dataConfig
                                    .CachedRemoveMethods
                                    .All(patternMethodName =>
                                                cachedRemovedMethodNames
                                                .Any(methodName => methodName.Contains(patternMethodName.CachedMethodName))
                                    );
                    }

                    return result;
                }, "Houve um erro na configuração de cached, o método de cache não localizado nas classes definidas");

            return services;
        }

        private static IEnumerable<string> GetMethodNames(this IEnumerable<Type> types)
            => types
                    .SelectMany(type =>
                                        type
                                        .GetMethods()
                                        .Select(method => $"{method.DeclaringType.FullName}.{method.Name}")
                                );
    }
}
