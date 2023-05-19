namespace API.DTOs
{
    public class EditAdminDto
    {
        public Guid user_id { get; set; }
        public String full_name { get; set; }

        public Guid? role_id { get; set; }
        public Guid? plant_id { get; set; }
        public String status { get; set; }
    }
}