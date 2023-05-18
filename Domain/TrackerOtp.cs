using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum TrackerOtpReason
    {
        FORGOT_PASSWORD=1,
        SIGN_IN=2,
        SIGN_UP=3

    }

    public enum TrackerOtpStatus
    {
        GENERATED=1,
        VERIFIED=2,
        FAILED=3
    }

    public class TrackerOtp
    {
        [Key]
        public Guid tracker_otp_id { get; set; }
        public String email { get; set; }
        public Guid user_id { get; set; }
        [Required]
        public String otp { get; set; }
        public TrackerOtpReason reason { get; set; } = TrackerOtpReason.SIGN_IN;
        public TrackerOtpStatus status { get; set; } = TrackerOtpStatus.GENERATED;
        public int failed_attempts { get; set; } = 0;
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_attempted_at { get; set; } = DateTime.Now;
    }
}

// tracker_otp_id	UUID | Primary Key
// email	TEXT | NULLABLE
// user_id	UUID
// otp	TEXT
// reason	enum ( FORGOT_PASSWORD=>1, SIGN_IN=>2, SIGN_UP=>3)
// status	"Enum ( GENERATED=>1
// VERIFIED =>2
// FAILED=>3 )"
// failed_attempts	Integer
// created_at	Datetime
// last_attempted_at	Datetime