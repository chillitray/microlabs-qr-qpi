using System.ComponentModel.DataAnnotations;
using Domain;

namespace API.DTOs
{
    public class SmtpDto
    {
        public Guid? smtp_config_id { get; set; }
        public int max_emails_per_day { get; set; } = 0;
        // [Required]
        public String email_id { get; set; } 
        // [Required]
        public String password { get; set; }
        public String email_type { get; set; }
    }
}