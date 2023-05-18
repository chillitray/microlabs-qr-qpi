using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingEditSmtpConfig
    {
        [Key]
        public Guid tracking_edit_smtp_config_id { get; set; }
        [Required]
        public String old_obj { get; set; } = "{}";
        [Required]
        public String new_obj { get; set; }
        public Guid smtp_config_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// tracking_edit_smtp_config_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// smtp_config_id	UUID
// user_id	UUID
// created_at	Datetime