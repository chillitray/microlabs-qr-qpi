namespace API.DTOs
{
    public class FetchUserNotificationDto
    {
        public Guid notification_activity_id { get; set; }
        public Guid notification_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime? first_read_at { get; set; }
        public int read_count { get; set; } = 0;
        public DateTime created_at { get; set; }
        public String notification_content { get; set; }
        public Guid notification_type { get; set; }
        public String redirect_url { get; set; }
        // public String status { get; set; }
        public String notification_type_type { get; set; }
        public String notification_priority { get; set; }
    }
}