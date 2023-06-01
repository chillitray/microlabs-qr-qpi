using System.Xml.Linq;
using API.Services;
using Domain;
using Persistence;

namespace API.Soap
{
    public class GenerateUrlsService : IGenerateUrlsService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        public GenerateUrlsService(DataContext context, IConfiguration config,IHttpContextAccessor httpContext)
        {

            this._httpContext = httpContext;
            this._config = config;
            // Console.WriteLine("COnstructor");
            this._context = context;
            // Console.WriteLine("COnstructor");
        }

        public string Test(string s)
        {
            // Console.WriteLine("Test Method Executed!");
            return s;
        }

        public void XmlMethod(XElement xml)
        {
            // Console.WriteLine(xml.ToString());
        }

        public List<String> GenerateShortUrls(String Token,int Quantity){
            // var ip = _
            var ip = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
            // Console.WriteLine(ip);
            var sessions = _context.PlantSessionManagement.Where(x => x.plant_access_token == Token & x.status==PlantSessionManagementStatus.GENERATED & x.expired_at >= DateTime.Now).ToList();
            if(sessions.Count <1){
                // Console.WriteLine("Hello - unauthorized");
                return new List<String>();
            }
            var session = sessions[0];
            // check the qr_limit of plant
            var plant = _context.Plant.Find(session.plant_id);
            if(plant==null){
                return new List<String>();
            }
            // find the number of qrs generated today for this product
            var qrs_generated = _context.QrManagement.Where(x => x.plant_id == session.plant_id & x.created_at >= DateTime.Today ).ToList().Count; 
            if(qrs_generated + Quantity > plant.plant_qr_limit){
                 return new List<String>();
            }
            
            // Console.WriteLine("Hello");
            //fetch the last_used_id 
            var last_used = _context.RangeTable.Where(x=>true).ToList().First();
            var last_used_id = last_used.last_used_value;
            if(last_used_id=="zzzzzzzzzz"){
                // Console.WriteLine("Hello - limit");
                return new List<String>();
            }

            //generate the links
            var utils = new Utils();
            List<string> urls = new List<string>();
            for(int i=0;i<Quantity;i++){
                last_used_id = utils.GenerateNextString(last_used_id);
                urls.Add(_config["ShortUrl"]+last_used_id);

                //insert record in Qr_Management
                _context.QrManagement.Add(
                    new QrManagement{
                        plant_id = session.plant_id,
                        created_at_ip=ip,
                        public_id = last_used_id
                    }
                );
            }
            last_used.last_used_value = last_used_id;
            session.last_access = DateTime.Now;
            session.last_access_ip = ip;
            session.status = PlantSessionManagementStatus.USED;
            _context.SaveChanges();
            // Console.WriteLine("Hello");

            return urls;

        }
    }
}