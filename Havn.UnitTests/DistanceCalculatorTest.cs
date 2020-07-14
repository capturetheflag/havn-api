using Havn.Calculations;
using Havn.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Havn.UnitTests
{
   [TestClass]
   public class DistanceCalculatorTest
   {
      private const double Epsilon = 0.01;

      [TestMethod]
      public void Should_Return_Valid_Distance_Between_Two_Different_Airports()
      {
         var distanceCalculator = new DistanceCalculator();
         var led = new Location
         {
            Latitude = 59.9311,
            Longitude = 30.270505
         };

         var lhr = new Location
         {
            Latitude = 51.469603,
            Longitude = -0.453566
         };

         var actualResult = distanceCalculator.GetDistanceMiles(led, lhr);
         var expectedResult = 1315.37;

         Assert.IsTrue(Math.Abs(actualResult - expectedResult) < Epsilon);
      }

      [TestMethod]
      public void Should_Return_Zero_Distance_Between_The_Same_Airport()
      {
         var distanceCalculator = new DistanceCalculator();
         var led = new Location
         {
            Latitude = 59.9311,
            Longitude = 30.270505
         };

         var distance = distanceCalculator.GetDistanceMiles(led, led);
         var result = Math.Round(distance, MidpointRounding.AwayFromZero);

         Assert.AreEqual(0, result);
      }
   }
}
