﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MHCache.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHCache.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : ActionFilterAttribute
    {
        private readonly int _timeToLiveSeconds;
        private const int TimeToLiveForThreeDays = 60 * 60 * 24 * 3;

        public IResponseCacheService ResponseCacheService { get; }

        public CachedAttribute(IResponseCacheService responseCacheService, int timeToLiveSeconds = TimeToLiveForThreeDays)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
            ResponseCacheService = responseCacheService;            
        }         

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await ResponseCacheService.GetCachedResponseAsStringAsync(cacheKey);

            //return cached value
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

            //get the new cache value by the current method, and set on redis database
            var executedContext = await next();

            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await ResponseCacheService.SetCacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
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
