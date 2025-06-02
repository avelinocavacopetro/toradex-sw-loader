using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("UserActivity")]
    public class UserActivity
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ActMessage { get; set; }

        [Required]
        public DateTime ActTimeStamp { get; set; }
    }
}
