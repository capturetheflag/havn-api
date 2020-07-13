using Havn.Api.Configuration;
using Havn.Calculations;
using Havn.DataProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace Havn.Api
{
   public class Startup
   {
      private const string AirportsServiceConfigName = "AirportsServiceConfig";

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

         services.AddControllers();

         services.AddOpenApiDocument();
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
         app.UseSwaggerUi3();
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
