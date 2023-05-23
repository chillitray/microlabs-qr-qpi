namespace API.DTOs
{
    public class NotificationTypeDto
    {
        public Guid? notification_type_id { get; set; }
        public String notifiication_type { get; set; }
        public String priority { get; set; }
        public String notification_for { get; set; }
        public String status { get; set; }
        public DateTime? created_at { get; set; } = DateTime.Now;
    }
}