using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserInformation
    {
        [Key]
        public Guid user_information_id { get; set; }
        [Required]
        public String full_name { get; set; }
        [Required]
        public String email { get; set; }
        public String phone_no { get; set; }
        [Required]
        public String country_code { get; set; }
        [Required]
        public String location_address { get; set; }
        [Required]
        public String location_city { get; set; }
        [Required]
        public String location_state { get; set; }
        [Required]
        public String location_country { get; set; }
        public String location_pincode { get; set; }
        public Guid product_id { get; set; }
        public Guid? qr_read_activity_id { get; set; }
        public Guid qr_id { get; set; }
        [Required]
        public String ip_address { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// user_information_id	UUID | Primary Key
// full_name	TEXT
// email	TEXT
// phone_no	TEXT | NULLABLE
// country_code	TEXT
// location_address	TEXT
// location_city	TEXT
// location_state	TEXT
// location_country	TEXT
// location_pincode	TEXT | NULLABLE
// product_id	UUID
// qr_read_activity_id	UUID | NULLABLE
// qr_id	UUID
// ip_address	Text
// created_at	Datetime