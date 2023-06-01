using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Trackers
{
    public class UserTrackerDto
    {
        public Guid edited_by_id { get; set; }
        public String edited_by_name { get; set; }
        public Guid user_id { get; set; }
        public String user_name { get; set; }
        public String action { get; set; }
        public Dictionary<String,Dictionary<String,String>> changes { get; set; }
    }
}