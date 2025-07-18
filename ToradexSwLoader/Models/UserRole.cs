using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}
