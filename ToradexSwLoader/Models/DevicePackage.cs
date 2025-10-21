using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class DevicePackage
    {
        [JsonPropertyName("component")]
        public string Component { get; set; } = null!;

        [JsonPropertyName("installed")]
        [NotMapped]
        public InstalledPackage Installed { get; set; } = null!;
    }
}
