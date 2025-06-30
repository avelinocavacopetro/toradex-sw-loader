using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Stack")]
    public class Stack
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        public ICollection<ProductStack> ProductStacks { get; set; } = new List<ProductStack>();
    }
}
