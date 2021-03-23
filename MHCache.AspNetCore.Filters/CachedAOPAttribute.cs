using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.Extensions;
using MHCache.Services;

namespace MHCache.AspNetCore.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CachedAOPAttribute : AbstractInterceptorAttribute
    {
        
        private const int TimeToLiveForThreeDays = 60 * 60 * 24 * 3;

        private int TimeToLiveSeconds { get; }      
        private string CacheName { get; set; }
        public IResponseCacheService ResponseCacheService { get; set; }

        public CachedAOPAttribute(
                                    //IResponseCacheService responseCacheService = null//,
                                    //string cacheName = null,
                                    //int timeToLiveSeconds = TimeToLiveForThreeDays
                                 )
        {
            //ResponseCacheService = responseCacheService;
            //TimeToLiveSeconds = timeToLiveSeconds;
            //CacheName = cacheName;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var key = context.GetGenerateKeyByMethodNameAndValues();
                        
            //Determine whether it is an asynchronous method
            bool isAsync = context.IsAsync();

            //First judge whether the method has a return value, and if not, do not make cache judgment|| method hasn't annotation
            var methodReturnType = context.ProxyMethod.ReturnType;
            if (
                    methodReturnType == typeof(void) || 
                    methodReturnType == typeof(Task) || 
                    methodReturnType == typeof(ValueTask) ||
                    context.ProxyMethod.GetCustomAttributes(GetType(), true).Length == 0
               )
            {
                await next(context);
                return;
            }

            var returnType = methodReturnType;
            if (isAsync)
            {
                //Gets the type of the asynchronous return
                returnType = returnType.GenericTypeArguments.FirstOrDefault();
            }
           
            //var cache = context.ServiceProvider.GetService();

            var cachedValue = await ResponseCacheService.GetCachedResponseAsStringAsync(key);

            if (string.IsNullOrWhiteSpace(cachedValue))
            {   
                var value = await JsonSerializer
                                    .DeserializeAsync(
                                        new MemoryStream(Encoding.UTF8.GetBytes(cachedValue)),
                                        returnType
                                    );

                context.SetReturnType(methodReturnType, returnType, value);

                return;
            }

            await next(context);
            
            await ResponseCacheService.SetCacheResponseAsync(key, context.GetReturnType(), TimeSpan.FromSeconds(TimeToLiveSeconds));
            
        }
    }
}
