using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Notification
    {
        [Key]
        public Guid notification_id { get; set; }
        [Required]
        public String notification_content { get; set; }
        public Guid notification_type { get; set; }
        [Required]
        public String redirect_url { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public DateTime created_at { get; set; } = DateTime.Now;
        public Guid created_by { get; set; }
        public DateTime last_updated_at { get; set; } = DateTime.Now;
        
    }
}


// notification_id	UUID | Primary Key
// notification_content	TEXT
// notification_type	notification_type_id
// redirect_url	TEXT
// status	"enum (
// Active=>1,                     Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime