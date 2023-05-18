using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum CounterfietTypeOptions
    {
        NUMBER_OF_SCANS=1,
        LOCATION=2,
        IP_ADDRESS=3

    }

    public class CounterfietManagement
    {
        [Key]
        public Guid counterfeit_mgmt_id { get; set; }
        public Guid product_id { get; set; }
        public CounterfietTypeOptions counterfeit_type { get; set; }
        public int low_risk_threshold { get; set; }
        public int moderate_threshold { get; set; }
        public int high_risk_threshold { get; set; }
        // public PlantStatusOptions status { get; set; }
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
    }
}

// counterfeit_mgmt_id	UUID | Primary Key
// product_id	UUID | unique
// counterfeit_type	enum ( NUMBER_OF_SCANS=>1, LOCATION=>2, IP_ADDRESS=>3 )
// low_risk_threshold	Integer
// moderate_threshold	Integer
// high_risk_threshold	Integer
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime