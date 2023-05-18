using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingCounterfeitManagement
    {
        [Key]
        public Guid tracking_counterfeit_id { get; set; }
        [Required]
        public String old_obj { get; set; } ="{}";
        [Required]
        public String new_obj { get; set; }
        public Guid counterfeit_mgmt_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// tracking_counterfeit_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// counterfeit_mgmt_id	UUID
// user_id	UUID
// created_at	Datetime