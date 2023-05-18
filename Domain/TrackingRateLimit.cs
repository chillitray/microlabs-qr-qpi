using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum RateLimitRequestType
    {
        OVERALL_DAY_IP=1,
        OVERALL_DAY_USER_ID=2,
        LOGIN_ATTEMPT_EMAIL=3
    }
    public class TrackingRateLimit
    {
        [Key]
        public Guid tracking_rate_limit_id { get; set; }
        [Required]
        public String request_obj { get; set; }
        public String response_obj { get; set; }
        public int current_count { get; set; } = 0;
        public RateLimitRequestType request_type { get; set; }
        [Required]
        public String unique_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;

    }
}


// tracking_rate_limit_id	UUID | Primary Key
// request_obj	Text
// response_obj	Text | NULLABLE
// current_count	Integer | 0
// request_type	enum (OVERALL_DAY_IP=1, OVERALL_DAY_USER_ID=>2, LOGIN_ATTEMPT_EMAIL=>3)
// unique_id	TEXT
// created_at	Datetime