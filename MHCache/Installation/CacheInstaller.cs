using MHCache.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MHCache.Installation
{
    public static class CacheInstaller
    {
        public static void InstallMHRedisCache(this IServiceCollection services, string connectionString)
        {
            //redis API, normally is used to heath checks
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
            
            //redis database connection
            services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
            
            //custom cache service
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();            
        }
    }
}
