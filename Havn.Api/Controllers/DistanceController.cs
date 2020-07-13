using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havn.Calculations;
using Havn.DataProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Havn.Api.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class DistanceController : ControllerBase
   {
      private readonly IAirportsDataProvider airportsDataProvider;
      private readonly IDistanceCalculator distanceCalculator;
      private readonly ILogger<DistanceController> _logger;

      public DistanceController(
         IAirportsDataProvider airportsDataProvider,
         IDistanceCalculator distanceCalculator,
         ILogger<DistanceController> logger)
      {
         this.airportsDataProvider = airportsDataProvider;
         this.distanceCalculator = distanceCalculator;
         _logger = logger;
      }

      [HttpGet]
      public async Task<double> Get(string from, string to)
      {
         var airportFrom = await this.airportsDataProvider.GetAirport(from);
         var airportTo = await this.airportsDataProvider.GetAirport(to);

         return this.distanceCalculator.GetDistanceMiles(
            airportFrom.Location,
            airportTo.Location);
      }
   }
}
