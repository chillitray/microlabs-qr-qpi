namespace Application.DTOs
{
    public class RoleDetailsDto
    {
        public RoleDetailsDto(Guid role_id, string role, string access_level)
        {
            this.role_id = role_id;
            this.role = role;
            this.access_level = access_level;
        }

        public Guid role_id { get; set; }
        public String role { get; set; }
        public String access_level { get; set; }
    }

    public class FetchAdminsDto
    {
        public Guid user_id { get; set; }        
        public String email { get; set; }        
        public String emp_id { get; set; }
        public String full_name { get; set; }
        public RoleDetailsDto role_details { get; set; }
        public String status { get; set; }
        public DateTime joined_date { get; set; }
        public DateTime? last_login_date { get; set; }
    }
}