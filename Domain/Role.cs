using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum StatusOptions
    {
        ACTIVE=1,
        INACTIVE=2
    } 

    public enum AccessLevelOptions
    {
        ADMIN=1,       
        PLANT_MANAGER=2
    }

    public class Role
    {   
        [Key]
        public Guid role_id { get; set; }
        [Required]
        public String role { get; set; }
        public AccessLevelOptions access_level { get; set; } = AccessLevelOptions.ADMIN;
        public DateTime created_at { get; set; } = DateTime.Now;
        public Guid? created_by { get; set; }
        public DateTime last_updated_at { get; set; } = DateTime.Now;
        public StatusOptions status { get; set; } = StatusOptions.ACTIVE;
    }
}


// role_id
// role
// access_level
// created_at
// created_by
// last_updated_at
// status