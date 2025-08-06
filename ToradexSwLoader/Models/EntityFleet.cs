using System.ComponentModel.DataAnnotations.Schema;

namespace ToradexSwLoader.Models
{
    [Table("EntityFleet")]
    public class EntityFleet
    {
        [ForeignKey("Entity")]
        public int EntityId { get; set; }
        public Entity? Entity { get; set; }

        [ForeignKey("Fleet")]
        public string FleetId { get; set; } = string.Empty;
        public Fleet? Fleet { get; set; }
    }
}
