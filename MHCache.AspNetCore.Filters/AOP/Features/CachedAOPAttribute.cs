using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.Extensions;
using MHCache.Features;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CachedAOPAttribute : AbstractInterceptorAttribute
    {
        
        private const int TimeToLiveForThreeDays = 60 * 60 * 24 * 3;

        public int TimeToLiveSeconds { get; set; } = TimeToLiveForThreeDays;
        public string CacheName { get; set; }

        private IResponseCacheService ResponseCacheService { get; set; }

        public CachedAOPAttribute()
        {
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var methodReturnType = context.ProxyMethod.ReturnType;
            if (
                    context.HasAttributeType(GetType()) ||
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
                                            TimeSpan.FromSeconds(TimeToLiveSeconds)
                                          );
            
        }
    }
}
