using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("GlobalFilterSettings")]
    public class GlobalFilterSettings
    {
        [Key]
        public int Id { get; set; }

        public string? SelectedDevice { get; set; }
        public int OnlineTime { get; set; }

        public string? SelectedPackage { get; set; }
        public string? Version { get; set; }

        public string? SelectedFleetsJson { get; set; }
        public string? SelectedProductsJson { get; set; }
        public string? SelectedDevicesJson { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
