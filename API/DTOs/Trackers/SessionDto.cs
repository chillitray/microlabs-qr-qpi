namespace API.DTOs.Trackers
{
    public class SessionDto
    {
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public DateTime last_login { get; set; }
        public String last_login_ip { get; set; }
        public DateTime last_access { get; set; }
        public String last_access_ip { get; set; }
        public String logout_status { get; set; }
        public DateTime? expired_at { get; set; }
    }
}