using Havn.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Havn.DataProviders
{
    public class AirportsDataProvider : IAirportsDataProvider
   {
      private const string ResourceName = "airports";
      private readonly HttpClient httpClient;

      public AirportsDataProvider(HttpClient httpClient)
      {
         this.httpClient = httpClient;
      }

      public async Task<Airport> GetAirport(string iataCode)
      {
         var result = await this.httpClient.GetStringAsync($"{ResourceName}/{iataCode.ToUpper()}");
         return JsonSerializer.Deserialize<Airport>(result);
      }
   }
}
