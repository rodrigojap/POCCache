using MHCache.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MHCache.Installation
{
    public static class CacheInstaller
    {
        public static IServiceCollection InstallMHRedisCache(this IServiceCollection services, IConfiguration configuration)
            => services
                    .GetConfigurationDataMHRedisCache(configuration)
                    .AddSingleton<IConnectionMultiplexer>(service =>
                    {                        
                        return ConnectionMultiplexer.Connect(configuration.GetValue("MHCache:RedisCacheOptions.Configuration", "localhost"));
                    })
                    .AddSingleton<IResponseCacheService, ResponseCacheService>();

        private static IServiceCollection GetConfigurationDataMHRedisCache(this IServiceCollection services, IConfiguration configuration) 
        {
            services
                .AddOptions<RedisCacheOptions>()
                .Configure(options => configuration.GetSection("MHCache:RedisCacheOptions").Bind(options));

            return services;
        }
    }
}
