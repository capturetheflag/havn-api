using System.Text.Json.Serialization;

namespace Havn.Models
{
   public struct Location
   {
      [JsonPropertyName("lat")]
      public double Latitude { get; set; }

      [JsonPropertyName("lon")]
      public double Longitude { get; set; }
   }
}
