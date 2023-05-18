using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum EmailTypeOptions
    {
        NOTIFICATION=1,
        REPLY=2,
        AUTHENTICATION=3
    } 

    public class SmtpConfig
    {
        [Key]
        public Guid smtp_config_id { get; set; }  
        public int max_emails_per_day { get; set; }
        [Required]
        public String email_id { get; set; } 
        [Required]
        public String password { get; set; }
        public EmailTypeOptions email_type { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
    }
}


// smtp_config_id	UUID | Primary Key
// max_emails_per_day	Integer
// email_id	Text
// password	Text
// email_type	"enum (
// NOTIFICATION=>1,        REPLY=> 2,   AUTHENTICATION=3 )"
// status	"enum (
// Active=>1,                     Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime