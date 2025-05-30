using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Device")]
    public class Device
    {
        [Key]
        public string DeviceUuid { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string DeviceName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string DeviceId { get; set; } = null!;

        public DateTime? LastSeen { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? ActivatedAt { get; set; }

        [Required]
        [MaxLength(15)]
        public string DeviceStatus { get; set; } = null!;

        [MaxLength(100)]
        public string? Notes { get; set; }

        public bool Hibernated { get; set; }
    }
}
