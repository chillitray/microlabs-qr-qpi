namespace API.DTOs
{
    public class CreateNotificationDto
    {
        public Guid? notification_id { get; set; }
        public String notification_content { get; set; }
        public Guid? notification_type { get; set; }
        public String redirect_url { get; set; }
        public String status { get; set; }
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public String notification_type_type { get; set; }
        public String notification_priority { get; set; }
        
    }
}