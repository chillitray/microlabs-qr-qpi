using System.ComponentModel.DataAnnotations;

namespace Domain
{

    public class ProductManagement
    {
        [Key]
        public Guid product_id { get; set; }
        [Required]
        public String product_name { get; set; }
        [Required]
        public String product_description { get; set; }
        public String product_logo { get; set; }
        [Required]
        public String product_writeup { get; set; }
        public int product_expiry_days { get; set; }
        public int product_mrp { get; set; }
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
        public PlantStatusOptions status { get; set; }

    }
}


// product_id	UUID | Primary Key
// product_name	Text
// product_description	Text
// product_logo	Text/URL | NULLABLE
// product_writeup	Text
// product_expiry_days	Integer
// product_mrp	Integer
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime
// status	"enum (
// Active=>1,        Inactive=> 2 )"