using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("DeviceProduct")]
    public class DeviceProduct
    {
        [ForeignKey("Device")]
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
