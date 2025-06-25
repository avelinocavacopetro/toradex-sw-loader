using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("ProductPackage")]
    public class ProductPackage
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Package")]
        public string PackageId { get; set; }
        public Package? Package { get; set; }
    }
}
