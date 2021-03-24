using System.Linq;
using AspectCore.DynamicProxy;
using MHCache.AspNetCore.Filters.AOP.DataModel;

namespace MHCache.AspNetCore.Filters.MVC.Extensions
{
    public static class AspectContextByConfigurationExtensions
    {
        public static MethodCacheConfiguration GetCacheConfigurationByMethodName(
                                                                   this AspectContext context,
                                                                   FilterCacheConfiguration optionsConfig//IOptionsMonitor<FilterCacheConfiguration> optionsConfig
                                                               )
        {
            var fullMethodName = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}";
            return optionsConfig.CachedRoutes.First(config => fullMethodName.Contains(config.CachedMethodName));
        }

        public static MethodCacheRemoveConfiguration GetCacheRemoveConfigurationByMethodName(
                                                                   this AspectContext context,
                                                                   FilterCacheConfiguration optionsConfig
                                                               )
        {
            var fullMethodName = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}";
            return optionsConfig.CacheRemoveRoutes.First(config => fullMethodName.Contains(config.CachedMethodName));
        }
    }
}
