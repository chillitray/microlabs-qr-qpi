

namespace API.DTOs.Trackers
{
    public class QrDto
    {
        public Guid qr_id { get; set; }
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public String action { get; set; }
        public Dictionary<String,Dictionary<String,String>> changes { get; set; } 
    }
}