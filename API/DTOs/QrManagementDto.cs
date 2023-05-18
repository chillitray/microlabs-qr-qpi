using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class QrManagementDto
    {
        public Guid qr_id { get; set; }
        public Guid? product_id { get; set; }
        public String product_name { get; set; }
        public Guid plant_id { get; set; }
        public String plant_name { get; set; }
        public String public_id { get; set; }
        public DateTime? manufactured_date { get; set; }
        public DateTime? expiry_date { get; set; }
        public int? product_mrp_copy { get; set; }
        public String pack_id { get; set; }
        public String serial_number { get; set; }
        public String batch_no { get; set; }
        public String status { get; set; }
        public String created_at_ip { get; set; }
        public Guid? updated_by { get; set; }
        public String updated_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_updated_at { get; set; }
    }
}