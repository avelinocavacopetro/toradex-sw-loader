using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Hardware")]
    public class Hardware
    {
        [Key]
        public int HardwareId { get; set; }  // auto increment

        public string HardwareName { get; set; }  // ex: "Apalis iMX8"

        public List<PackageHardware> PackageHardwares { get; set; } = new List<PackageHardware>();
    }
}
