using Havn.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Havn.DataProviders
{
   public class AirportsDataProvider : IAirportsDataProvider
   {
      private const string ResourceName = "airports";
      private readonly IDistributedCache cache;
      private readonly HttpClient httpClient;
      private readonly ILogger<AirportsDataProvider> logger;

      public AirportsDataProvider(
         IDistributedCache cache,
         HttpClient httpClient,
         ILogger<AirportsDataProvider> logger)
      {
         this.cache = cache;
         this.httpClient = httpClient;
         this.logger = logger;
      }

      public async Task<Airport> GetAirport(string iataCode)
      {
         iataCode = iataCode.ToUpper();

         var cachedAirport = await this.GetCachedAirport(iataCode);
         if (cachedAirport != null)
         {
            return cachedAirport;
         }

         var response = await this.httpClient.GetAsync($"{ResourceName}/{iataCode.ToUpper()}");

         if (response.StatusCode == HttpStatusCode.NotFound)
         {
            return null;
         }

         var httpResult = await response.Content.ReadAsStringAsync();
         this.CacheValue(iataCode, httpResult);

         return JsonSerializer.Deserialize<Airport>(httpResult);
      }

      private async Task<Airport> GetCachedAirport(string key)
      {
         try
         {
            var cachedValue = await this.cache.GetStringAsync(key);
            return cachedValue == null ? null : JsonSerializer.Deserialize<Airport>(cachedValue);
         }
         catch (Exception ex)
         {
            this.logger.LogError(ex, $"Failed to get a value from cache with key: {key}");
            return null;
         }
      }

      private async void CacheValue(string key, string value)
      {
         var cacheOptions = new DistributedCacheEntryOptions();
         cacheOptions.SetSlidingExpiration(TimeSpan.FromDays(30));

         try
         {
            await this.cache.SetStringAsync(key, value);
         }
         catch (Exception ex)
         {
            this.logger.LogError(ex, $"Failed to add new cache value with key: {key}");
         }
      }
   }
}
