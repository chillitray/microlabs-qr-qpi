using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PlantSessionFetchDto
    {
        public Guid plant_session_id { get; set; }
        public Guid? plant_id { get; set; }
        public String plant_key { get; set; }
        public String plant_access_token { get; set; }
        public String status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? expired_at { get; set; }
        public DateTime? last_access { get; set; }
        public String last_access_ip { get; set; }
    }
}