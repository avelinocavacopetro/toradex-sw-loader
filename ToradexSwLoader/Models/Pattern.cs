using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Pattern")]
    public class Pattern
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NameContains { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}