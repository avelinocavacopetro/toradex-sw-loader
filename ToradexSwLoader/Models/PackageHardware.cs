using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("PackageHardware")]
    public class PackageHardware
    {
        [ForeignKey("Package")]
        public string PackageId { get; set; } = string.Empty;
        public Package? Package { get; set; }

        [ForeignKey("HardwareId")]
        public int HardwareId { get; set; }
        public Hardware? Hardware { get; set; }
    }
}
