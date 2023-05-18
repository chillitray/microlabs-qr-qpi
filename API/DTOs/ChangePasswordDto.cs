using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public String newPassword { get; set; }

        [Required]
        public String oldPassword { get; set; }
    }
}