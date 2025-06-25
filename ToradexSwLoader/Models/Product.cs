using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(30)]
        public string ProductName { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        public ICollection<ProductPackage> ProductPackages { get; set; } = new List<ProductPackage>();
        public ICollection<DeviceProduct> DeviceProducts { get; set; } = new List<DeviceProduct>();
    }
}
