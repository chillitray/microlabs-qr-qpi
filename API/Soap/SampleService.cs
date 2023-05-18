using System.Xml.Linq;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Soap
{
    public class SampleService : ISampleService
    {
        private readonly DataContext _context;
        // private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        public SampleService(DataContext context, IHttpContextAccessor httpContext)
        {
            this._httpContext = httpContext;
            // this._config = config;
            this._context = context;
            Console.WriteLine("COnstructor");
        }

        public string Test(string s)
        {
            Console.WriteLine("Test Method Executed!");
            return s;
        }

        public void XmlMethod(XElement xml)
        {
            Console.WriteLine(xml.ToString());
        }

        public PlantSessionDto PlantLogin(String Key, String PlantId)
        {
            var ip = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();

            // Console.WriteLine("Helloooo");
            var plant_id = new Guid(PlantId);
            var plantKey = _context.PlantKeyManagement.Where(x => x.plant_key==Key & x.plant_id==plant_id).ToList();
            // Console.WriteLine("Helloooo23");
            if(plantKey.Count < 1){
                return new PlantSessionDto{
                    token = "Error"
                }; 
            }
            // Console.WriteLine("Helloooo65");
            
            // check for previous sessions
            var sessions = _context.PlantSessionManagement.Where(x => x.plant_id == plant_id & x.status==PlantSessionManagementStatus.GENERATED).ToList();
            foreach(PlantSessionManagement session in sessions){
                session.status=PlantSessionManagementStatus.EXPIRED;
                session.last_access = DateTime.Now;
                session.last_access_ip = ip;
            }

            //generate a token
            var utils = new Utils();
            var token = utils.GenerateRandomOTP(2);

            //create new record in db
            _context.PlantSessionManagement.Add(
                new PlantSessionManagement{
                    plant_id = plant_id,
                    plant_key = Key,
                    plant_access_token = token,
                    expired_at = DateTime.Now.AddMinutes(5),
                    last_access_ip = ip
                }
            );
            _context.SaveChanges();

            return new PlantSessionDto{
                token = token
            };
        }
    }
}