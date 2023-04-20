using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum StatusOptions
    {
        ACTIVE=1,
        INACTIVE=2
    } 

    public class Role
    {   
        [Key]
        public Guid role_id { get; set; }
        [Required]
        public String role { get; set; }
        public DateTime created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime last_updated_at { get; set; }
        public StatusOptions status { get; set; }
    }
}