
namespace API.DTOs
{
    public class CounterfietDto
    {
        public Guid? counterfeit_mgmt_id { get; set; }
        public Guid? product_id { get; set; }
        public String product_name { get; set; }
        public String counterfeit_type { get; set; }
        public int low_risk_threshold { get; set; }
        public int moderate_threshold { get; set; }
        public int high_risk_threshold { get; set; }
        // public Guid? created_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? last_updated_at { get; set; }
    }
}