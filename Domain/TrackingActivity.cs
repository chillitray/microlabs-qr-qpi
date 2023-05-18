using System.ComponentModel.DataAnnotations;
namespace Domain
{
    public enum SeverityType{
        NON_CRITICAL=1,
        SEMI_CRITICAL=2,
        CRITICAL=3
    }
    public class TrackingActivity
    {
        [Key]
        public Guid tracking_activity_id { get; set; }
        [Required]
        public String custom_obj { get; set; }
        [Required]
        public String message { get; set; }
        public SeverityType severity_type { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// tracking_activity_id	UUID | Primary Key
// custom_obj	Text
// message	TEXT
// severity_type	enum(NON-CRITICAL=>1, SEMI-CRITICAL=>2, CRITICAL=>3)
// user_id	UUID
// created_at	Datetime