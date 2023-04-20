

using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum RequestProductAccessStatus
    {
        PENDING=1,
        ACCEPTED=2,
        REJECTED=3
    }

    public class RequestProductAccess
    {
        [Key]
        public Guid request_product_access_id { get; set; }
        public Guid product_id { get; set; }
        public Guid plant_id { get; set; }
        public String message { get; set; }
        public int requested_qr_limit { get; set; }
        public RequestProductAccessStatus status { get; set; }
        public Guid requested_by_user_id { get; set; }
        public Guid responded_by_user_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
        
    }
}

// request_product_access_id	UUID | Primary Key
// product_id	UUID
// plant_id	UUID
// message	TEXT | NULLABLE
// requested_qr_limit	Integer
// status	"enum (
// PENDING=>1,        ACCEPTED=> 2, REJECTED=>3 )"
// requested_by_user_id	UUID
// responded_by_user_id	UUID
// created_at	Datetime
// last_updated_at	Datetime