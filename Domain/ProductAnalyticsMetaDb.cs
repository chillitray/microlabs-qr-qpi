using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductAnalyticsMetaDb
    {
     [Key]
     public Guid product_analytics_meta_db { get; set; } 
     public Guid product_id { get; set; }  
     public int qr_generated { get; set; } = 0;
     public int qr_printed { get; set; } = 0;
     public int qr_scanned { get; set; } = 0;
     public int counterfeit_scans { get; set; } = 0;
     public DateTime created_at { get; set; } = DateTime.Now;
     public DateTime updated_at { get; set; } = DateTime.Now;

    }
}