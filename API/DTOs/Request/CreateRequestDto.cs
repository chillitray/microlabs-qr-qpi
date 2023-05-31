namespace API.DTOs.Request
{
    public class CreateRequestDto
    {
        public Guid? request_product_access_id { get; set; }
        public Guid? product_id { get; set; }
        public String product_name { get; set; }
        
        public Guid? plant_id { get; set; }
        public String plant_name { get; set; }
        public String plant_code { get; set; }
        public String message { get; set; }
        public int requested_qr_limit { get; set; } = 0;
        public String status { get; set; }
        public Guid? requested_by_user_id { get; set; }
        public String  requested_by_user_name { get; set; }
        public Guid? responded_by_user_id { get; set; }
        public String  responded_by_user_name { get; set; }
        public DateTime? created_at { get; set; } = DateTime.Now;
        public DateTime? last_updated_at { get; set; } = DateTime.Now;
    }
}