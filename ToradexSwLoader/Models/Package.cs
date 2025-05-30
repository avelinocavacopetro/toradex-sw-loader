using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    [Table("Package")]
    public class Package
    {
        [Key]
        [Required]
        [MaxLength(200)]
        [JsonPropertyName("packageId")]
        public string PackageId { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string PackageName { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonPropertyName("version")]
        public string PackageVersion { get; set; }
    }
}
