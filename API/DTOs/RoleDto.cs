using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RoleDto
    {
        public Guid? role_id { get; set; }
        public String role { get; set; }
        public  String access { get; set; }
        public DateTime? created_at { get; set; }
        public String status { get; set; }
    }
}