using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("FinalProduct")]
    public class FinalProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string DeviceUuid { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? LastSeen { get; set; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Package")]
        public string PackageId { get; set; } = string.Empty;
        public Package? Package { get; set; }

        [ForeignKey("Stack")]
        public int StackId { get; set; }
        public Stack? Stack { get; set; }

        [ForeignKey("Fleet")]
        public string FleetId { get; set; } = string.Empty;
        public Fleet? Fleet { get; set; }

        [ForeignKey("UserPetrotec")]
        public int UserId { get; set; }
        public UserPetrotec? UserPetrotec { get; set; }

        public bool Enabled { get; set; }
    }
}
