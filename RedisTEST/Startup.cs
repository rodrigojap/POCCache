using MHCache.AspNetCore.Filters.AOP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisTEST.Services;

namespace RedisTEST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //custom cache installation, we can put other parameters if we want
            services.AddScoped<IWeatherForecastService, WeatherForecastService>();
            //services.InstallMHRedisCacheFilters(Configuration);
            services.InstallRedisAOPCacheFilter(Configuration);
                           
            services.AddControllers();
            //services
            //    .AddControllers(
            //        options => options
            //                    .InstallMHRedisCacheFilter()
            //                    .InstallMHRedisCacheRemoveFilter()
            //    );
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
