using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class loginDto
    {
        public String Email { get; set; }
        public String Password { get; set; }
        public String Otp { get; set; } = null;
    }
}