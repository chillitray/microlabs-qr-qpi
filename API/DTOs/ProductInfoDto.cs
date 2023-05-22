namespace API.DTOs
{
    public class ProductInfoDto
    {
        // public Guid qr_id { get; set; }
        // public Guid? product_id { get; set; }
        public String product_name { get; set; }
        // public Guid plant_id { get; set; }
        public String plant_name { get; set; }
        public String public_id { get; set; }
        public DateTime? manufactured_date { get; set; }
        public DateTime? expiry_date { get; set; }
        public int? product_mrp_copy { get; set; }
        public String pack_id { get; set; }
        public String serial_number { get; set; }
        public String batch_no { get; set; }
        public String product_logo { get; set; }
        public String product_writeup { get; set; }
        public String plant_location_geo { get; set; }
    }
}