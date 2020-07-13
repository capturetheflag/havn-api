using Havn.Models;
using System;

namespace Havn.Calculations
{
   public class DistanceCalculator : IDistanceCalculator
   {
      public double GetDistanceMiles(Location location1, Location location2)
      {
         // Haversine Formula
         // https://en.wikipedia.org/wiki/Haversine_formula

         const double R = 3958.8;

         var lat1 = location1.Latitude;
         var lon1 = location1.Longitude;
         var lat2 = location2.Latitude;
         var lon2 = location2.Longitude;

         var dLat = Deg2Rad(lat2 - lat1);
         var dLon = Deg2Rad(lon2 - lon1);
         var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
               Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                  Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

         var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
         var distance = R * c;

         return distance;
      }

      private double Deg2Rad(double degrees)
      {
         return degrees * (Math.PI / 180);
      }
   }
}
