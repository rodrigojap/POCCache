using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using MHCache.Extensions;
using MHCache.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MHCache.AspNetCore.Filters.MVC.Extensions
{
    public class CachedAOPByConfiguration : AbstractInterceptorAttribute
    {
        
        private const int TimeToLiveForThreeDays = 60 * 60 * 24 * 3;

        public int TimeToLiveSeconds { get; set; } = TimeToLiveForThreeDays;
        public string CacheName { get; set; }

        private IResponseCacheService ResponseCacheService { get; set; }

        public CachedAOPByConfiguration()
        {
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cachedConfigurationProvider = context.ServiceProvider.GetService<IOptionsMonitor<FilterCacheConfiguration>>();
            var configCache = context.GetCacheConfigurationByMethodName(cachedConfigurationProvider.CurrentValue);
            
            var methodReturnType = context.ProxyMethod.ReturnType;
            if (
                    configCache == null ||
                    methodReturnType == typeof(void) || 
                    methodReturnType == typeof(Task) || 
                    methodReturnType == typeof(ValueTask)
               )
            {
                await next(context);
                return;
            }

            if (string.IsNullOrWhiteSpace(CacheName))
            {
                CacheName = context.GetGenerateKeyByMethodNameAndValues();
            }

            ResponseCacheService = context.ServiceProvider.GetService<IResponseCacheService>();

            var returnType = context.IsAsync() ? methodReturnType.GenericTypeArguments.FirstOrDefault() : methodReturnType;
           
            var cachedValue = await ResponseCacheService.GetCachedResponseAsync(CacheName, returnType);

            if (cachedValue != null)
            {
                context.SetReturnType(methodReturnType, cachedValue);
                return;
            }

            await next(context);
                                    
            await ResponseCacheService
                    .SetCacheResponseAsync(
                                            CacheName, 
                                            await context.GetReturnValueAsync(), 
                                            TimeSpan.FromSeconds(configCache.TimeToLiveSeconds ?? TimeToLiveSeconds)
                                          );
            
        }
    }
}
