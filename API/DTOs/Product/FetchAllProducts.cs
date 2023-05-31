

namespace API.DTOs.Product
{
    public class FetchAllProducts
    {
        public Guid product_id { get; set; }
        public String product_name { get; set; }
        public String status { get; set; }
    }
}