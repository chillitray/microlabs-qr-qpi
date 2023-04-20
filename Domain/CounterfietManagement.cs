using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class CounterfietManagement
    {
        [Key]
        public Guid counterfeit_mgmt_id { get; set; }
        public Guid product_id { get; set; }
        public int no_risk_threshold { get; set; }
        public int moderate_threshold { get; set; }
        public int high_risk_threshold { get; set; }
        public PlantStatusOptions status { get; set; }
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
    }
}

// counterfeit_mgmt_id	UUID | Primary Key
// product_id	UUID | unique
// no_risk_threshold	Integer
// moderate_threshold	Integer
// high_risk_threshold	Integer
// status	"enum (
// Active=>1,        Inactive=> 2 )"
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime