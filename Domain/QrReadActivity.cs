using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class QrReadActivity
    {
        [Key]
        public Guid qr_read_activity_id { get; set; }
        public Guid qr_id { get; set; }
        public Guid product_id { get; set; }
        [Required]
        public String ip_address { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// qr_read_activity_id	UUID | Primary Key
// qr_id	UUID
// product_id	UUID
// ip_address	Text
// created_at	Datetime