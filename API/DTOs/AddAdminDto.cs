using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddAdminDto
    {
        [Required]
        public String emp_id { get; set; }
        public String full_name { get; set; }

        public Guid role_id { get; set; }
        public Guid? plant_id { get; set; }
        public String Email { get; set; }
    }
}