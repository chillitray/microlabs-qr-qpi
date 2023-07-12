using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum PlantStatusOptions
    {
        ACTIVE=1,
        INACTIVE=2
    } 

    public class Plant
    {
        [Key]
        public Guid plant_id { get; set; }
        [Required]
        public String plant_name { get; set; }
        [Required]
        public String plant_code { get; set; }
        [Required]
        public String plant_description { get; set; }
        [Required]
        public String plant_location_address { get; set; }
        // [Required]
        // public String plant_location_city { get; set; }
        // [Required]
        // public String plant_location_state { get; set; }
        // [Required]
        // public String plant_location_country { get; set; }
        // [Required]
        // public String plant_location_pincode { get; set; }
        // [Required]
        // public String plant_location_geo { get; set; }
        public int plant_qr_limit { get; set; }
        public Guid created_by { get; set; }
        public Guid? operated_id { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public DateTime founded_on { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
    }
}

// plant_id	UUID | Primary Key
// plant_name	Text
// plant_code	Text | Unique
// plant_description	Text
// plant_location_address	Text
// plant_location_city	Text
// plant_location_state	Text
// plant_location_country	Text
// plant_location_pincode	Text
// plant_location_geo	Text
// plant_qr_limit	Integer
// created_by	UUID
// status	"enum (
// Active=>1,        Inactive=> 2 )"
// founded_on	Datetime
// created_at	Datetime
// last_updated_at	Datetime