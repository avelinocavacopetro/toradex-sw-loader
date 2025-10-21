using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class InstalledPackage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("version")]
        public string Version { get; set; } = null!;

        [JsonPropertyName("packageId")]
        public string PackageId { get; set; } = null!;
    }
}
