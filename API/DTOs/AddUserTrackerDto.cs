
namespace API.DTOs
{
    public class AddUserTrackerDto
    {
        public Guid user_id { get; set; }
        public String emp_id { get; set; }
        public String full_name { get; set; }
        public Guid role_id { get; set; }
        // public Guid? plant_id { get; set; }
        public String Email { get; set; }
        public DateTime joined_date { get; set; }
        public DateTime last_updated_at { get; set; }
        public String status { get; set; }
        public DateTime created_at { get; set; }
        public Guid? created_by { get; set; }
    }
}