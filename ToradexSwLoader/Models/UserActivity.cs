using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("UserActivity")]
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserPetrotec? UserPetrotec { get; set; } 

        [Required]
        [MaxLength(500)]
        public string ActMessage { get; set; } = string.Empty;

        [Required]
        public DateTime ActTimeStamp { get; set; }
    }
}
