using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class DeviceDTO
    {
        [JsonPropertyName("packageIds")]
        public List<string>? PackageIds { get; set; }

        [JsonPropertyName("devices")]
        public List<string>? Devices { get; set; }
    }
}
