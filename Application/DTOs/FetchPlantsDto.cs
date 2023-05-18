using Domain;

namespace Application.DTOs
{
    public class FetchPlantsDto
    {
        public String plant_name { get; set; }
        public String plant_code { get; set; }
        public String plant_location_address { get; set; }
        public String plant_location_city { get; set; }
        public String plant_location_state { get; set; }
        public String plant_location_country { get; set; }
        public int plant_qr_limit { get; set; }
        // public PlantStatusOptions plant_status { get; set; }
        public Guid plant_id { get; set; }
        public Guid admin_user_id { get; set; }
        public String admin_name { get; set; }
        public String admin_emp_id { get; set; }
        public String admin_email { get; set; }
    }
}