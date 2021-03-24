using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHCache.AspNetCore.Filters.MVC.DataModel;
using MHCache.Extensions;
using MHCache.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RemoveCacheByConfigurationAttribute : ActionFilterAttribute
    {
        private static readonly string[] AllowedRequestMethod = { "POST", "PUT", "PATCH", "DELETE" };
       
        private readonly IEnumerable<RouteCacheRemoveConfiguration> _removeCacheRoutes;

        public IResponseCacheService ResponseCacheService { get; }

        public RemoveCacheByConfigurationAttribute(
                                IResponseCacheService responseCacheService,
                                IOptionsMonitor<FilterCacheConfiguration> configuration
                              )
        {
            var configValues = configuration.CurrentValue;
            _removeCacheRoutes = configValues.CacheRemoveRoutes;
            ResponseCacheService = responseCacheService;            
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var removeRouteCache = _removeCacheRoutes
                                    .FirstOrDefault(
                                        cacheRoute => request.Path.ToString()
                                                        .Contains(cacheRoute.CachedRouteName, StringComparison.InvariantCultureIgnoreCase)
                                    );

            var isAllowedMethod = AllowedRequestMethod
                                .Any(allowMethod => allowMethod.Equals(request.Method, StringComparison.InvariantCultureIgnoreCase));

            await next();

            if (isAllowedMethod && removeRouteCache != null)
            {
                var qq = ResponseCacheService.GetAllKeys();
                await ResponseCacheService.RemoveAllByPattern(removeRouteCache.PatternRouteName);
                var q = ResponseCacheService.GetAllKeys();
            }
        }                
    }
}
