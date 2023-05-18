using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class NotificationActivity
    {
        [Key]
        public Guid notification_activity_id { get; set; }
        public Guid notification_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime first_read_at { get; set; }
        public int read_count { get; set; } = 0;
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;

    }
}

// notification_activity_id	UUID | Primary Key
// notification_id	UUID
// user_id	UUID
// first_read_at	Datetime
// read_count	Integer
// created_at	Datetime
// last_updated_at	Datetime