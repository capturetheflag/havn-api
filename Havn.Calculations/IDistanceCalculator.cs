using Havn.Models;

namespace Havn.Calculations
{
   public interface IDistanceCalculator
   {
      double GetDistanceMiles(Location location1, Location location2);
   }
}
