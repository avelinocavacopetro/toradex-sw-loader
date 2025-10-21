using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("StackPackage")]
    public class StackPackage
    {
        [ForeignKey("Stack")]
        public int StackId { get; set; }
        public Stack? Stack { get; set; }

        [ForeignKey("Package")]
        public string PackageId { get; set; } = string.Empty;
        public Package? Package { get; set; }
    }
}
