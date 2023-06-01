namespace API.DTOs.Trackers
{
    public class PlantDto
    {
        public Guid plant_id { get; set; }
        public String plant_name { get; set; }
        public String plant_code { get; set; }
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public String action { get; set; }
        public Dictionary<String,Dictionary<String,String>> changes { get; set; }
    }
}