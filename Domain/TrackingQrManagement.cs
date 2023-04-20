using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingQrManagement
    {
        [Key]
        public Guid tracking_qr_id { get; set; }
        [Required]
        public String old_obj { get; set; }
        [Required]
        public String new_obj { get; set; }
        public Guid qr_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; }
    }
}

// tracking_qr_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// qr_id	UUID
// user_id	UUID
// created_at	Datetime