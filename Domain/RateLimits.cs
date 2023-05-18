using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum RateTypeOptions
    {
        PARTICULAR_IP =1,
        PARTICULAR_USER_ID=2,
        MAX_LOGIN_FAILED_ATTEMPTS=3
    }

    public class RateLimits
    {
        [Key]
        public Guid rate_limit_id { get; set; }
        public RateTypeOptions rate_type { get; set; }
        public int max_allowed_per_day { get; set; }
        public int? max_allowed_overall { get; set; }
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
    }
}

// rate_limit_id	UUID | Primary Key
// rate_type	enum( Whetever API/overall limit we are using/ admin requires)
// max_allowed_per_day	Integer
// max_allowed_overall	Integer | NULLABLE
// status	"enum (
// Active=>1,                     Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime