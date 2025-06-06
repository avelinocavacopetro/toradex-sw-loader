using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    [Table("Fleet")]
    public class Fleet
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        [JsonPropertyName("name")]
        public string FleetName { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        private string fleetType = string.Empty;

        [Required]
        public string FleetType
        {
            get => fleetType;
            set
            {
                if (!string.Equals(value, "static", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(value, "dynamic", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("FleetType deve ser 'static' ou 'dynamic'");
                }
                fleetType = value.ToLower(); 
            }
        }
        public string? Expression { get; set; }
    }
}
