using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum TrackerEmailReason
    {
        OTP_FORGOT_PASSWORD_VERIFY=1,
        SIGN_IN=2,
        SIGN_UP=3   
    }

    public enum TrackerEmailStatus
    {
        GENERATED=1,
        SENT=2,
        NOT_DELIVERED=3
    }


    public class TrackerEmail
    {
        [Key]
        public Guid tracker_email_id { get; set; }
        [Required]
        public String email { get; set; }
        [Required]
        public String email_subject { get; set; }
        [Required]
        public String email_body { get; set; }
        public TrackerEmailReason reason { get; set; } = TrackerEmailReason.SIGN_IN;
        public TrackerEmailStatus status { get; set; } = TrackerEmailStatus.GENERATED;
        public Guid user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        
    }
}

// tracker_email_id	UUID | Primary Key
// email	TEXT
// email_subject	TEXT
// email_body	TEXT
// reason	enum ( OTP_FORGOT_PASSWORD_VERIFY=>1, SIGN_IN=>2, SIGN_UP=>3)
// status	"enum (
// 1 -> Generated
// 2 -> Sent    3->NotDelivered
// )"
// user_id	UUID
// created_at	Datetime