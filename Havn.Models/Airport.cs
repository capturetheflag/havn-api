using System.Text.Json.Serialization;

namespace Havn.Models
{
   public class Airport
   {
      [JsonPropertyName("iata")]
      public string Iata { get; set; }

      [JsonPropertyName("location")]
      public Location Location { get; set; }
   }
}
