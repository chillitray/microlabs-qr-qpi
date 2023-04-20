using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class TrackingProductActivity
    {
        [Key]
        public Guid tracking_product_id { get; set; }
        [Required]
        public String old_obj { get; set; }
        [Required]
        public String new_obj { get; set; }
        public Guid product_id { get; set; }
        public Guid user_id { get; set; }
        public DateOnly created_at { get; set; }
    }
}

// tracking_product_id	UUID | Primary Key
// old_obj	Text
// new_obj	Text
// product_id	UUID
// user_id	UUID
// created_at	Datetime