using Havn.Models;
using System.Threading.Tasks;

namespace Havn.DataProviders
{
   public interface IAirportsDataProvider
   {
      Task<Airport> GetAirport(string iataCode);
   }
}