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
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string FleetType { get; set; } = string.Empty;
        public string? Expression { get; set; }
    }
}
