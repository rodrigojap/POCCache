using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MHCache.AspNetCore.Filters.MVC.Extensions
{
    public class CachedRemoveAOPByConfiguration : AbstractInterceptorAttribute
    {
        private IResponseCacheService ResponseCacheService { get; set; }

        public CachedRemoveAOPByConfiguration()
        {
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cachedRemoveConfigurationProvider = context.ServiceProvider.GetService<IOptionsMonitor<FilterCachedConfiguration>>();
            var configCacheRemove = context.GetCacheRemoveConfigurationByMethodName(cachedRemoveConfigurationProvider.CurrentValue);
            if (configCacheRemove != null)
            {
                ResponseCacheService = context.ServiceProvider.GetService<IResponseCacheService>();
                await ResponseCacheService
                    .RemoveAllByPatternAsync(configCacheRemove.PatternMethodCachedName);
            }

            await next(context);            
        }
    }
}
