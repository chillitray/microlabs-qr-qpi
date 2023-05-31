
namespace API.DTOs
{
    public class CreateUserInfoDto
    {
        public Guid? user_information_id { get; set; }
        public String full_name { get; set; }
        public String email { get; set; }
        public String phone_no { get; set; }
        public String country_code { get; set; }
        public String location_address { get; set; }
        public String location_city { get; set; }
        public String location_state { get; set; }
        public String location_country { get; set; }
        public String location_pincode { get; set; }
        public Guid? product_id { get; set; }
        public String product_name { get; set; }
        public Guid? qr_read_activity_id { get; set; }
        public Guid? qr_id { get; set; }
        public String qr_public_id { get; set; }
        public String ip_address { get; set; }
        public DateTime? created_at { get; set; } = DateTime.Now;
    }
}