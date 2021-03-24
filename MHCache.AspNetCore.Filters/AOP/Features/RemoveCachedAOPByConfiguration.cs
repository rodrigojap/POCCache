using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MHCache.AspNetCore.Filters.MVC.Extensions
{
    public class RemoveCachedAOPByConfiguration : AbstractInterceptorAttribute
    {
        private IResponseCacheService ResponseCacheService { get; set; }

        public RemoveCachedAOPByConfiguration()
        {
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cachedRemoveConfigurationProvider = context.ServiceProvider.GetService<IOptionsMonitor<FilterCacheConfiguration>>();
            var configCacheRemove = context.GetCacheRemoveConfigurationByMethodName(cachedRemoveConfigurationProvider.CurrentValue);
            if (configCacheRemove != null)
            {
                ResponseCacheService = context.ServiceProvider.GetService<IResponseCacheService>();
                await ResponseCacheService
                    .RemoveAllByPattern(configCacheRemove.PatternMethodCachedName);
            }

            await next(context);            
        }
    }
}
