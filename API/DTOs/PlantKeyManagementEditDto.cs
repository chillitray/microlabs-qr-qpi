namespace API.DTOs
{
    public class PlantKeyManagementEditDto
    {
        public Guid plant_key_id { get; set; }
        public Guid? plant_id {get; set;}
        public String plant_key { get; set; }

        public String status { get; set; } = null;
        public String plant_name { get; set; }
    }
}