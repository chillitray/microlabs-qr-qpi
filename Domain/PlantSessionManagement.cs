using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum PlantSessionManagementStatus
    {
        GENERATED=1,
        USED= 2,
        EXPIRED=3
    }

    public class PlantSessionManagement
    {
        [Key]
        public Guid plant_session_id { get; set; }
        public Guid plant_id { get; set; }
        [Required]
        public String plant_key { get; set; }
        [Required]
        public String plant_access_token { get; set; }
        public PlantSessionManagementStatus status { get; set; } = PlantSessionManagementStatus.GENERATED;
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime expired_at { get; set; }
        public DateTime last_access { get; set; } = DateTime.Now;
        public String last_access_ip { get; set; }
        
    }
}

// plant_session_id	UUID | Primary Key
// plant_id	UUID
// plant_key	Text
// plant_access_token	Text
// status	"enum (
// GENERATED=>1,                     USED=> 2, EXPIRED=>3 )"
// created_at	Datetime
// expired_at	Datetime
// last_access	Datetime
// last_access_ip	Text