
namespace API.DTOs.Trackers
{
    public class TrackingActivityDto
    {
        public String message { get; set; }
        public String severity_type { get; set; }
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public DateTime created_at { get; set; }
    }
}