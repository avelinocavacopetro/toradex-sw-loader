using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Hardware")]
    public class Hardware
    {
        [Key]
        public int HardwareId { get; set; } 

        public string HardwareName { get; set; } = string.Empty;

        public List<PackageHardware> PackageHardwares { get; set; } = new List<PackageHardware>();
    }
}
