using Havn.Models;
using System.Net;
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
         var response = await this.httpClient.GetAsync($"{ResourceName}/{iataCode.ToUpper()}");

         if (response.StatusCode == HttpStatusCode.NotFound)
         {
            return null;
         }

         var result = await response.Content.ReadAsStringAsync();
         return JsonSerializer.Deserialize<Airport>(result);
      }
   }
}
