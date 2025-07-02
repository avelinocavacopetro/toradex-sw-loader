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
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [NotMapped]
        public List<string> HardwareIds { get; set; } = new List<string>();

        public List<PackageHardware> PackageHardwares { get; set; } = new List<PackageHardware>();

        [NotMapped]
        public List<string> HardwareNames => PackageHardwares?
            .Select(ph => ph.Hardware.Name)
            .ToList() ?? new List<string>();

        public ICollection<StackPackage> StackPackages { get; set; } = new List<StackPackage>();
    }
}
