using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum QrManagementStatus
    {
        CREATED=1,
        PRINTED=2,
        BLOCKED=3,
        COUNTERFEIT_BLOCKED=4
    }


    public class QrManagement
    {
        [Key]
        public Guid qr_id { get; set; }
        public Guid? product_id { get; set; }
        public Guid plant_id { get; set; }
        [Required]
        public String public_id { get; set; }
        public DateTime? manufactured_date { get; set; }
        public DateTime? expiry_date { get; set; }
        public int? product_mrp_copy { get; set; }
        public String pack_id { get; set; }
        public String serial_number { get; set; }
        public String batch_no { get; set; }
        public QrManagementStatus status { get; set; } = QrManagementStatus.CREATED;
        [Required]
        public String created_at_ip { get; set; }
        public Guid? updated_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
        
    }
}

// qr_id	UUID | Primary Key
// product_id	UUID | NULLABLE
// plant_id	UUID
// public_id	UUID | UNIQUE
// manufactured_date	Datetime | NULLABLE
// expiry_date	Datetime | NULLABLE
// product_mrp_copy	Integer | NULLABLE
// pack_id	TEXT | NULLABLE
// serial_number	TEXT | NULLABLE
// batch_no	TEXT | NULLABLE
// status	"enum (
// CREATED=>1,        PRINTED=> 2, BLOCKED=>3, COUNTERFEIT_BLOCKED=>4 )"
// plant_session_id	UUID
// created_at_ip	TEXT
// updated_by	UUID | NUlLABLE
// created_at	Datetime
// last_updated_at	Datetime