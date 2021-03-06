using System.Linq;
using AspectCore.DynamicProxy;
using MHCache.AspNetCore.Filters.AOP.DataModel;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    public static class AspectContextByConfigurationExtensions
    {
        public static MethodCachedConfiguration GetCacheConfigurationByMethodName(
                                                                   this AspectContext context,
                                                                   FilterCachedConfiguration optionsConfig
                                                               )
        {
            var fullMethodName = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}";
            return optionsConfig
                    .CachedMethods?
                    .FirstOrDefault(
                            config => fullMethodName
                                        .Contains(
                                            config.CachedMethodName, 
                                            System.StringComparison.InvariantCultureIgnoreCase
                                        )
                    );
        }

        public static MethodCachedRemoveConfiguration GetCacheRemoveConfigurationByMethodName(
                                                                   this AspectContext context,
                                                                   FilterCachedConfiguration optionsConfig
                                                               )
        {
            var fullMethodName = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}";
            return optionsConfig
                    .CachedRemoveMethods?
                    .FirstOrDefault(
                            config => fullMethodName
                                        .Contains(
                                            config.CachedMethodName,
                                            System.StringComparison.InvariantCultureIgnoreCase
                                        )
                    );
        }
    }
}
