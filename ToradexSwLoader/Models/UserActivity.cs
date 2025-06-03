using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("UserActivity")]
    public class UserActivity
    {
        [Key]
        public int UserActId { get; set; }

        [Required]
        public int UserLoginId { get; set; }

        [ForeignKey(nameof(UserLoginId))]
        public LoginLog LoginLog { get; set; }

        [Required]
        [MaxLength(500)]
        public string ActMessage { get; set; }

        [Required]
        public DateTime ActTimeStamp { get; set; }
    }
}
