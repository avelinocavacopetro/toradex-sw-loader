using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
   [Table("UserPetrotec")]
   public class UserPetrotec
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserRoleId))]
        public int UserRoleId { get; set; }
        public UserRole? UserRole { get; set; }

        [ForeignKey(nameof(EntityId))]
        public int? EntityId { get; set; }
        public Entity? Entity { get; set; }
        public bool Enabled { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Culture { get; set; } = string.Empty;  
    }
}
