using Havn.Calculations;
using Havn.DataProviders;
using Havn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Havn.Api.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class DistanceController : ControllerBase
   {
      private const int IataCodeLength = 3;
      private readonly IAirportsDataProvider airportsDataProvider;
      private readonly IDistanceCalculator distanceCalculator;
      private readonly ILogger<DistanceController> logger;

      public DistanceController(
         IAirportsDataProvider airportsDataProvider,
         IDistanceCalculator distanceCalculator,
         ILogger<DistanceController> logger)
      {
         this.airportsDataProvider = airportsDataProvider;
         this.distanceCalculator = distanceCalculator;
         this.logger = logger;
      }

      [HttpGet]
      [Produces("application/json")]
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

         Airport airportFrom = null;
         Airport airportTo = null;
         try
         {
            var airportFromTask = this.airportsDataProvider.GetAirport(from);
            var airportToTask = this.airportsDataProvider.GetAirport(to);

            Task.WaitAll(new[] { airportFromTask, airportToTask });

            airportFrom = airportFromTask.Result;
            airportTo = airportToTask.Result;
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

         var distance = this.distanceCalculator.GetDistanceMiles(
               airportFrom.Location,
               airportTo.Location);
         
         return this.Ok(JsonSerializer.Serialize(distance));
      }

      private bool IsValid(string iataCode)
      {
         return iataCode?.Length == IataCodeLength;
      }
   }
}
