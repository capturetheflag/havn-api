using System;
using System.Threading.Tasks;
using Havn.Calculations;
using Havn.DataProviders;
using Havn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Havn.Api.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class DistanceController : ControllerBase
   {
      private const int IataCodeLength = 3;

      private readonly IDistributedCache distributedCache;
      private readonly IAirportsDataProvider airportsDataProvider;
      private readonly IDistanceCalculator distanceCalculator;
      private readonly ILogger<DistanceController> logger;

      public DistanceController(
         IDistributedCache distributedCache,
         IAirportsDataProvider airportsDataProvider,
         IDistanceCalculator distanceCalculator,
         ILogger<DistanceController> logger)
      {
         this.distributedCache = distributedCache;
         this.airportsDataProvider = airportsDataProvider;
         this.distanceCalculator = distanceCalculator;
         this.logger = logger;
      }

      [HttpGet]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<double>> Get(string from, string to)
      {
         if (!this.IsValid(from) || !this.IsValid(to))
         {
            return this.BadRequest();
         }

         from = from.ToUpper();
         to = to.ToUpper();

         var cachedValue =
            (await this.distributedCache.GetStringAsync($"{from}-{to}")) ??
            await this.distributedCache.GetStringAsync($"{to}-{from}");

         double distance = 0;
         if (!string.IsNullOrEmpty(cachedValue) && double.TryParse(cachedValue, out distance))
         {
            return this.Ok(distance);
         }

         Airport airportFrom = null;
         Airport airportTo = null;
         try
         {
            airportFrom = await this.airportsDataProvider.GetAirport(from);
            airportTo = await this.airportsDataProvider.GetAirport(to);
         }
         catch (Exception ex)
         {
            this.logger.LogError(ex, $"Failed to get the airport information for either {from} or {to}");
            throw;
         }

         if (airportFrom == null || airportTo == null)
         {
            return this.BadRequest($"Airport {(airportFrom == null ? from : to)} does not exist in the system");
         }

         distance = this.distanceCalculator.GetDistanceMiles(
               airportFrom.Location,
               airportTo.Location);

         this.CacheDistance($"{from}-{to}", distance);
         this.CacheDistance($"{to}-{from}", distance);
         
         return this.Ok(distance);
      }

      private async void CacheDistance(string key, double value)
      {
         var cacheOptions = new DistributedCacheEntryOptions();
         cacheOptions.SetSlidingExpiration(TimeSpan.FromDays(30));

         try
         {
            await this.distributedCache.SetStringAsync(key, value.ToString());
         }
         catch (Exception ex)
         {
            this.logger.LogError(ex, $"Failed to add new cache value with key: {key}");
         }
      }

      private bool IsValid(string iataCode)
      {
         return iataCode?.Length == IataCodeLength;
      }
   }
}
