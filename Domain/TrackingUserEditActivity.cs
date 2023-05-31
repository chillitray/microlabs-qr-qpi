using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingUserEditActivity
    {
        [Key]
        public Guid tracking_user_edit_id { get; set; }
        [Required]
        public String old_obj { get; set; } = "{}";
        [Required]
        public String new_obj { get; set; }
        public Guid user_id { get; set; }
        public Guid edited_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}

// tracking_user_edit_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// user_id	UUID
// created_at	Datetime
