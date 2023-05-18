using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingPlantActivity
    {
        [Key]
        public Guid tracking_plant_id { get; set; }
        [Required]
        public String old_obj { get; set; } = "{}";
        [Required]
        public String new_obj { get; set; }
        public Guid plant_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// tracking_plant_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// plant_id	UUID
// user_id	UUID
// created_at	Datetime