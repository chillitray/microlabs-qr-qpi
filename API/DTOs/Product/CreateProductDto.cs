

namespace API.DTOs.Product
{
    public class CreateProductDto
    {
        public Guid? product_id { get; set; }
        public String product_name { get; set; }
        public String product_description { get; set; }
        public String product_logo { get; set; }
        public String product_writeup { get; set; }
        public int product_expiry_days { get; set; } = 0;
        public int product_mrp { get; set; } = 0;
        public String status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? last_updated_at { get; set; }
        public int no_of_plants { get; set; } = 0;
        public int no_qrs_generated { get; set; } = 0;
        public int no_qrs_printed { get; set; } = 0;
        public int no_qrs_unused { get; set; } = 0;
        public int no_qrs_scanned { get; set; } = 0;
        public int no_qrs_counterfied { get; set; } = 0;
        public int no_qrs_disabled { get; set; } = 0;
        
    }
}