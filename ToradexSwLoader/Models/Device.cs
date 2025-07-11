using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Device")]
    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; } 
        public ICollection<DeviceProduct> DeviceProducts { get; set; } = new List<DeviceProduct>();
    }
}
