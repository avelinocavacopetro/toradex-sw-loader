using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Device")]
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
    }
}
