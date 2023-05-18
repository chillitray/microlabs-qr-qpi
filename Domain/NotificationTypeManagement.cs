using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum NotificationPriority
    {
        LOW = 1,
        MODERATE =2 ,
        HIGH=3
    }
    public class NotificationTypeManagement
    {
        [Key]
        public Guid notification_type_id { get; set; }
        [Required]
        public String notifiication_type { get; set; }
        public NotificationPriority priority { get; set; }
        public String notification_for { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
    }
}

// notification_type_id	UUID | Primary Key
// notifiication_type	TEXT | UNIQUE
// priority	"enum (
// LOW=>1,                     MODERATE=> 2,             HIGH=>3 )"
// notification_for	TEXT | NULLABLE
// status	"enum (
// Active=>1,                     Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime