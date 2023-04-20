using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingProductPlantMapActivity
    {
        [Key]
        public Guid tracking_product_plant_map_id { get; set; }
        [Required]
        public String old_obj { get; set; }
        [Required]
        public String new_obj { get; set; }
        public Guid product_plant_mapping_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; }
    }
}

// tracking_product_plant_map_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// product_plant_mapping_id	UUID
// user_id	UUID
// created_at	Datetime