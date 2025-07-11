using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class PackageUri
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
