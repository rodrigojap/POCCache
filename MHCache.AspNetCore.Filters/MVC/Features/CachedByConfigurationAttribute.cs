using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHCache.AspNetCore.Filters.MVC.DataModel;
using MHCache.Extensions;
using MHCache.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedByConfigurationAttribute : ActionFilterAttribute
    {
        private const int TimeToLiveForThreeDays = 60 * 60 * 24 * 3;
        private const string AllowedRequestMethod = "GET";

        private readonly int _timeToLiveSeconds;        
        private readonly IEnumerable<RouteCachedConfiguration> _cachedRoutes;

        public IResponseCacheService ResponseCacheService { get; }

        public CachedByConfigurationAttribute(
                                IResponseCacheService responseCacheService,
                                IOptionsMonitor<FilterCachedConfiguration> configuration,
                                int timeToLiveSeconds = TimeToLiveForThreeDays
                              )
        {
            var configValues = configuration.CurrentValue;
            _timeToLiveSeconds = configValues.GeneralTimeToLiveSeconds ?? timeToLiveSeconds;
            _cachedRoutes = configValues.CachedRoutes;
            ResponseCacheService = responseCacheService;            
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var routeCache = _cachedRoutes
                                .FirstOrDefault(
                                    cacheRoute => request.Path.ToString()
                                                    .Contains(cacheRoute.CachedRouteName, StringComparison.InvariantCultureIgnoreCase)
                                );

            if (request.Method.Equals(AllowedRequestMethod, StringComparison.InvariantCultureIgnoreCase) && routeCache != null)
            {
                var cacheKey = GenerateCacheKeyFromRequest(request);

                var cachedResponse = await ResponseCacheService.GetCachedResponseAsStringAsync(cacheKey);
                
                if (!string.IsNullOrEmpty(cachedResponse))
                {
                    var contentResult = new ContentResult
                    {
                        Content = cachedResponse,
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                    context.Result = contentResult;
                    return;
                }

                var executedContext = await next();

                if (executedContext.Result is OkObjectResult okObjectResult)
                {                    
                    await ResponseCacheService
                            .SetCacheResponseAsync(
                                                    cacheKey, 
                                                    okObjectResult.Value, 
                                                    TimeSpan.FromSeconds( routeCache.TimeToLiveSeconds ??_timeToLiveSeconds)
                                                  );
                }
            }
            else 
            {
                await next();
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
