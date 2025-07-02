using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        public ICollection<DeviceProduct> DeviceProducts { get; set; } = new List<DeviceProduct>();
        public ICollection<ProductStack> ProductStacks { get; set; } = new List<ProductStack>();
    }
}
