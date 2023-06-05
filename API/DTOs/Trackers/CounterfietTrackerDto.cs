namespace API.DTOs.Trackers
{
    public class CounterfietTrackerDto
    {
        public Guid counterfeit_mgmt_id { get; set; }
        public Guid product_id { get; set; }
        public String product_name { get; set; }
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public String action { get; set; }
        public Dictionary<String,Dictionary<String,String>> changes { get; set; } 
    }
}