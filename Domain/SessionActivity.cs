using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SessionActivity
    {
        [Key]
        public Guid session_id { get; set; }
        public Guid user_id { get; set; }
        [Required]
        public String user_access_token { get; set; }
        public DateTime last_login { get; set; } = DateTime.Now;
        [Required]
        public String last_login_ip { get; set; }
        public DateTime last_access { get; set; } = DateTime.Now;
        [Required]
        public String last_access_ip { get; set; }
    }
}

// session_id	UUID | Primary Key
// user_id	UUID
// user_access_token	Text
// last_login	Datetime
// last_login_ip	Text
// last_access	Datetime
// last_access_ip	Text