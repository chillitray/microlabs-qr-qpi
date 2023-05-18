using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public enum USerStatusOptions
    {
        VERIFICATION_PENDING=0,
        ACTIVE=1,
        BLOCKED=2
    } 

    public class User : IdentityUser
    {
        
        public Guid user_id { get; set; } = Guid.NewGuid();
        // [Required]
        // public String email { get; set; }

        [Required]
        public String emp_id { get; set; }
        public DateTime joined_date { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;

        // [Required]
        // public String password { get; set; }

        public String full_name { get; set; }

        public Guid role_id { get; set; }

        public USerStatusOptions status { get; set; } = USerStatusOptions.VERIFICATION_PENDING;
        public Guid? plant_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public Guid? created_by { get; set; }
    }
}


// user_id	UUID | Primary key
// email	Text | Unique
// emp_id	Text | Unique
// joined_date	Datetime
// last_updated_at	Datetime
// password	Text
// full_name	Text | NULLABLE
// role_id	UUID
// status	"Enum (
//     Verification_Pending=>0*,        1- Active, 
//     2- Blocked, 
// )"
// plant_id	UUID | NULL
// created_at	Datetime
// created_by	UUID | NULL