using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class DeviceFleet
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
