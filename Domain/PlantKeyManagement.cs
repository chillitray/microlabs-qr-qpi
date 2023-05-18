using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class PlantKeyManagement
    {
        [Key]
        public Guid plant_key_id { get; set; }
        public Guid plant_id { get; set; }
        [Required]
        public String plant_key { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
        
    }
}

// plant_key_id	UUID | Primary Key
// plant_id	UUID
// plant_key	Text
// status	"enum (
// Active=>1,                     Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime