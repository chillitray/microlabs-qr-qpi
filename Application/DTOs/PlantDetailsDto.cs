using Domain;

namespace Application.DTOs
{
    public class ProductDetails
    {
        public ProductDetails(Guid product_id, string product_name)
        {
            this.product_id = product_id;
            this.product_name = product_name;
        }

        public Guid product_id { get; set; }
        public String product_name { get; set; }
    }

    public class PlantDetailsDto
    {
        public PlantDetailsDto(Guid plant_id, string plant_name, string plant_code, string plant_description, string plant_location_address, string plant_location_city, string plant_location_state, string plant_location_country, string plant_location_pincode, string plant_location_geo, int plant_qr_limit, string created_by_name, Guid created_by, Guid? operated_id, string operated_by_name, String status, DateTime founded_on, DateTime created_at, DateTime last_updated_at, List<ProductDetails> productDetails)
        {
            this.plant_id = plant_id;
            this.plant_name = plant_name;
            this.plant_code = plant_code;
            this.plant_description = plant_description;
            this.plant_location_address = plant_location_address;
            this.plant_location_city = plant_location_city;
            this.plant_location_state = plant_location_state;
            this.plant_location_country = plant_location_country;
            this.plant_location_pincode = plant_location_pincode;
            this.plant_location_geo = plant_location_geo;
            this.plant_qr_limit = plant_qr_limit;
            this.created_by_name = created_by_name;
            this.created_by = created_by;
            this.operated_id = operated_id;
            this.operated_by_name = operated_by_name;
            this.status = status;
            this.founded_on = founded_on;
            this.created_at = created_at;
            this.last_updated_at = last_updated_at;
            this.productDetails = productDetails;
        }

        public Guid plant_id { get; set; }
        public String plant_name { get; set; }
        public String plant_code { get; set; }
        public String plant_description { get; set; }
        public String plant_location_address { get; set; }
        
        public String plant_location_city { get; set; }
        
        public String plant_location_state { get; set; }
        
        public String plant_location_country { get; set; }
        
        public String plant_location_pincode { get; set; }
        
        public String plant_location_geo { get; set; }
        public int plant_qr_limit { get; set; }
        public String created_by_name { get; set; }
        public Guid created_by { get; set; }
        public Guid? operated_id { get; set; }
        public String operated_by_name { get; set; }
        public String status { get; set; }
        public DateTime founded_on { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }

        public List<ProductDetails> productDetails { get; set; }
    }
}