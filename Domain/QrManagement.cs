using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum QrManagementStatus
    {
        CREATED=1,
        PRINTED=2,
        BLOCKED=3,
        INVALID=4
    }


    public class QrManagement
    {
        [Key]
        public Guid qr_id { get; set; }
        public Guid product_id { get; set; }
        public Guid plant_id { get; set; }
        public Guid public_id { get; set; }
        public DateTime manufactured_date { get; set; }
        public DateTime expiry_date { get; set; }
        [Required]
        public String batch_no { get; set; }
        public QrManagementStatus status { get; set; }
        [Required]
        public String created_at_ip { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
        
    }
}

// qr_id	UUID | Primary Key
// product_id	UUID
// plant_id	UUID
// public_id	UUID | UNIQUE
// manufactured_date	Datetime
// expiry_date	Datetime
// batch_no	TEXT
// status	"enum (
// CREATED=>1,        PRINTED=> 2, BLOCKED=>3, INVALID=> 4 )"
// created_at_ip	TEXT
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime