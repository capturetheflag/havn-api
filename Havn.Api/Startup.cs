using Havn.Api.Configuration;
using Havn.Calculations;
using Havn.DataProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Net.Http;

namespace Havn.Api
{
   public class Startup
   {
      private const string AirportsServiceConfigName = "AirportsServiceConfig";

      private const string RedisConfigName = "RedisConfiguration";

      private const int HttpHandlerLifetimeHours = 2;

      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services
            .AddHttpClient<IAirportsDataProvider, AirportsDataProvider>(
               (sp, client) => ConfigureAirportsServiceClient(client, this.Configuration))
            .SetHandlerLifetime(TimeSpan.FromHours(HttpHandlerLifetimeHours));

         services.AddScoped<IDistanceCalculator, DistanceCalculator>();
         ConfigureApplicationCache(services, this.Configuration);

         services.AddControllers();

         services.AddOpenApiDocument(settings =>
         {
            settings.Title = "Havn API";
            settings.Description = "RESTful API endpoint which provides distance (in miles) between two airports";
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseHttpsRedirection();

         app.UseRouting();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });

         app.UseOpenApi();
         app.UseSwaggerUi3(c =>
         {
            c.DocumentTitle = "Havn API";
         });
      }

      private static void ConfigureApplicationCache(IServiceCollection services, IConfiguration configuration)
      {
         var cacheConfiguration = new CacheConfiguration();
         configuration.GetSection(RedisConfigName).Bind(cacheConfiguration);
         if (cacheConfiguration.UseRedis)
         {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(cacheConfiguration.RedisEndpointAddress));
            services.AddStackExchangeRedisCache(options => options.Configuration = cacheConfiguration.RedisEndpointAddress);
         }
         else
         {
            services.AddDistributedMemoryCache();
         }
      }

      private static HttpClient ConfigureAirportsServiceClient(HttpClient httpClient, IConfiguration configuration)
      {
         var airportsServiceConfig = new ExternalRestApiServiceConfiguration();
         configuration.GetSection(AirportsServiceConfigName).Bind(airportsServiceConfig);

         httpClient.BaseAddress = new Uri(airportsServiceConfig.EndpointAddress);
         httpClient.Timeout = TimeSpan.FromSeconds(airportsServiceConfig.TimeoutSeconds);

         return httpClient;
      }
   }
}
