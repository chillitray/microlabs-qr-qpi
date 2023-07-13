namespace API.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public String Token { get; set; }
        public String UserName { get; set; }
        public String UserEmail { get; set; }
        public String EmpId { get; set; }
        public String Role { get; set; }
        public Guid? RoleId { get; set; }

    }
}