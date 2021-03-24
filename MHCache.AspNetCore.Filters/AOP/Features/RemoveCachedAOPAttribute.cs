using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.Extensions;
using MHCache.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RemoveCachedAOPAttribute : AbstractInterceptorAttribute
    {
        public string RemovePattern { get; set; }

        private IResponseCacheService ResponseCacheService { get; set; }

        public RemoveCachedAOPAttribute()
        {
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (context.HasAttributeType(GetType()))
            {
                await next(context);
                return;
            }

            await next(context);
                                    
            await ResponseCacheService
                    .RemoveAllByPattern(RemovePattern);
            
        }
    }
}
