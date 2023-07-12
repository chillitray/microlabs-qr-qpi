using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class PlantDto
    {
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
        public Guid? operated_id { get; set; }
        public DateTime founded_on { get; set; }
    }
}