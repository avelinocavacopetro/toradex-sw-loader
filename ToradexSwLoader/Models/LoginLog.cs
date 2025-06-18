using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("LoginLog")]
    public class LoginLog
    {
        [Key]
        public int LoginId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserPetrotec? UserPetrotec { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime LoginTimeStamp { get; set; }
    }
}
