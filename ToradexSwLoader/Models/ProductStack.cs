using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("ProductStack")]
    public class ProductStack
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Stack")]
        public int StackId { get; set; }
        public Stack? Stack { get; set; }
    }
}
