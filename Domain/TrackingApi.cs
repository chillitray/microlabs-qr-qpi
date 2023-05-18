using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public enum RequestMethods
    {
        GET=1,
        POST=2,
        PUT=3,
        DELETE=4
    }
    public enum RequestType
    {
        USER=1,
        PLANT_KEY=2,
        PLANT_TOKEN=3
    }

    public class TrackingApi
    {
        [Key]
        public Guid tracking_api_id { get; set; }
        public RequestMethods req_method { get; set; }
        [Required]
        public String req_endpoint { get; set; }
        [Required]
        public String req_headers { get; set; }
        [Required]
        public String request_obj { get; set; }
        public String response_obj { get; set; }
        public RequestType request_type { get; set; }
        public String ip_address { get; set; }
        public String unique_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        
    }
}


// tracking_api_id	UUID | Primary Key
// req_method	enum ( GET=>1, POST=>2, PUT=>3, DELETE=>4)
// req_endpoint	TEXT
// req_headers	TEXT
// request_obj	Text
// response_obj	Text | NULLABLE
// request_type	enum (USER=1, PLANT_KEY=>2, PLANT_TOKEN=>3)
// ip_address	TEXT | NULLABLE
// unique_id	TEXT | NULLABLE
// created_at	Datetime