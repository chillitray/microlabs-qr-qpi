using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public enum ProductMediaTypeOptions
    {
        IMAGE=1,
        VIDEO=2,
        IFRAME=3
    }
    public class ProductMedia
    {
        [Key]
        public Guid product_media_id { get; set; }
        [Required]
        public String url { get; set; }
        public ProductMediaTypeOptions media_type { get; set; }
        public Guid product_id { get; set; }
        public Guid created_by { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime last_updated_at { get; set; } = DateTime.Now;
        public PlantStatusOptions status { get; set; } = PlantStatusOptions.ACTIVE;
        
    }
}

// product_media_id	UUID | Primary Key
// url	Text
// media_type	"enum (
// IMAGE=>1,        VIDEO=> 2, IFRAME=>3 )"
// product_id	UUID
// created_by	UUID
// created_at	Datetime
// last_updated_at	Datetime
// status	"enum (
// Active=>1,        Inactive=> 2 )"