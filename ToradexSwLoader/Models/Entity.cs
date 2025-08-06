using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("Entity")]
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Observation { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        public ICollection<EntityFleet> EntityFleets { get; set; } = new List<EntityFleet>();
    }
}
