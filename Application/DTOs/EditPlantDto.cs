
namespace Application.DTOs
{
    public class EditPlantDto
    {
        public Guid plant_id { get; set; }
        public String plant_name { get; set; }
        public String plant_code { get; set; }
        public String plant_description { get; set; }
        public String plant_location_address { get; set; }
        public String plant_location_city { get; set; }
        public String plant_location_state { get; set; }
        public String plant_location_country { get; set; }
        public String plant_location_pincode { get; set; }
        public String plant_location_geo { get; set; }
        public int plant_qr_limit { get; set; }
        public Guid? operated_id { get; set; }
        public DateTime? founded_on { get; set; }
    }
}