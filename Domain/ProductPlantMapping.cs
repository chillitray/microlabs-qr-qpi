using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductPlantMapping
    {
        [Key]
        public Guid product_plant_mapping_id { get; set; }
        public Guid product_id { get; set; }
        public Guid plant_id { get; set; }
        public int max_qr_limit { get; set; }
        public PlantStatusOptions status { get; set; }
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
        
    }
}

// product_plant_mapping_id	UUID | Primary Key
// product_id	UUID
// plant_id	UUID
// max_qr_limit	Integer
// status	"enum (
// Active=>1,        Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime