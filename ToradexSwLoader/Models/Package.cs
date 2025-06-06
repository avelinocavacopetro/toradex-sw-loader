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
        public string PackageId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string PackageName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [JsonPropertyName("version")]
        public string PackageVersion { get; set; } = string.Empty;

        [NotMapped]
        public List<string> HardwareIds { get; set; } = new List<string>();

        public List<PackageHardware> PackageHardwares { get; set; } = new List<PackageHardware>();

        [NotMapped]
        public List<string> HardwareNames => PackageHardwares?
            .Select(ph => ph.Hardware.HardwareName)
            .ToList() ?? new List<string>();
    }
}
