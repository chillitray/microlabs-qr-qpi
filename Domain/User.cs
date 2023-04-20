using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum USerStatusOptions
    {
        ACTIVE=1,
        DEACTIVATE=2,
        BLOCKED=3
    } 

    public class User
    {
        [Key]
        public Guid user_id { get; set; }
        [Required]
        public String email { get; set; }

        [Required]
        public String emp_id { get; set; }
        public DateTime joined_date { get; set; }
        public DateTime last_updated_at { get; set; }

        [Required]
        public String password { get; set; }

        public String full_name { get; set; }

        [Key]
        public Guid role_id { get; set; }

        public USerStatusOptions status { get; set; }
    }
}