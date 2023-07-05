using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using API.Services;
using API.Trackers;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class QrManagementController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public QrManagementController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(QrManagementFetchDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify whether the plant had a key already
            var QrKeys = _context.QrManagement.Where(x=>x.qr_id == managementDto.qr_id).ToList();
            if(QrKeys.Count<1){
                return NotFound("Invalid qr_id");
            }
            var QrKey = QrKeys[0];
            //format the data to string
            // Console.WriteLine(QrKey.qr_id);
            var tracker = new TrackerUtils();
            var old_obj_db = new AddQrTrackerDto{
                qr_id = QrKey.qr_id,
                // product_id = QrKey.product_id,
                // plant_id = QrKey.plant_id,
                // public_id = QrKey.public_id,
                // manufactured_date = QrKey.manufactured_date,
                // expiry_date = QrKey.expiry_date,
                // product_mrp_copy = QrKey.product_mrp_copy,
                // pack_id =QrKey.pack_id,
                // serial_number =QrKey.serial_number,
                // batch_no =QrKey.batch_no,
                status =QrKey.status.ToString(),
                // created_at_ip = QrKey.created_at_ip,
                // updated_by =QrKey.updated_by ?? new Guid(),
                // created_at=QrKey.created_at,
                // last_updated_at=QrKey.last_updated_at
            };
            var old_obj_string = tracker.CreateQrManagementObj(old_obj_db);

            var type_dict = new Dictionary<string, QrManagementStatus>(){
                {"PRINTED", QrManagementStatus.PRINTED},
                {"BLOCKED", QrManagementStatus.BLOCKED},
                {"COUNTERFEIT_BLOCKED", QrManagementStatus.COUNTERFEIT_BLOCKED}
            };

            QrKey.status = type_dict[managementDto.status];
            QrKey.updated_by = logged_user.user_id;
            QrKey.last_updated_at = DateTime.Now;

            //format the data to string
            var new_obj_db = new AddQrTrackerDto{
                qr_id = QrKey.qr_id,
                // product_id = QrKey.product_id,
                // plant_id = QrKey.plant_id,
                // public_id = QrKey.public_id,
                // manufactured_date = QrKey.manufactured_date,
                // expiry_date = QrKey.expiry_date,
                // product_mrp_copy = QrKey.product_mrp_copy,
                // pack_id =QrKey.pack_id,
                // serial_number =QrKey.serial_number,
                // batch_no =QrKey.batch_no,
                status =QrKey.status.ToString(),
                // created_at_ip = QrKey.created_at_ip,
                // updated_by =QrKey.updated_by ?? new Guid(),
                // created_at=QrKey.created_at,
                // last_updated_at=QrKey.last_updated_at
            };
            var new_obj_string = new TrackerUtils().CreateQrManagementObj(new_obj_db);

            //add record in tracker
            _context.TrackingQrManagement.Add(
                new TrackingQrManagement{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    qr_id = QrKey.qr_id,
                    user_id = logged_user.user_id
                }
            );

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("Status not updated");
            }

            return Ok("Status updated successfully");
            

        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<QrManagementDto>>> FetchSmtpRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.QrManagement.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid> plant_ids = new List<Guid>();
            List<Guid?> product_ids = new List<Guid?>();
            List<Guid?> user_ids = new List<Guid?>();
            
            foreach(QrManagement plt in email){

                    if (plt.product_id != null){
                        product_ids.Add(plt.product_id);
                    }
                    if(plt.updated_by != null){
                        user_ids.Add(plt.updated_by);
                    }
                    plant_ids.Add(plt.plant_id);
                    
            }

            //fetch the plant details
            var plants = _context.Plant.Where(x => plant_ids.Contains(x.plant_id)).ToList();
            //fetch the product details
            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();            
            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();

            List<QrManagementDto> details_final = new List<QrManagementDto>();
            foreach(QrManagement em in email){
                var data=new QrManagementDto{
                        qr_id = em.qr_id,
                        product_id = em.product_id,
                        plant_id = em.plant_id,
                        public_id = em.public_id,
                        manufactured_date = em.manufactured_date,
                        expiry_date = em.expiry_date,
                        product_mrp_copy = em.product_mrp_copy,
                        pack_id = em.pack_id,
                        serial_number = em.serial_number,
                        batch_no = em.batch_no,
                        status = em.status.ToString(),
                        created_at_ip = em.created_at_ip,
                        updated_by = em.updated_by,
                        created_at = em.created_at,
                        last_updated_at = em.last_updated_at
                    };

                foreach(Plant plt in plants){
                    if(plt.plant_id == em.plant_id){
                        data.plant_name = plt.plant_name;
                    }
                }

                if(em.product_id !=null){
                    foreach(ProductManagement product in products){
                        if(em.product_id == product.product_id){
                            data.product_name = product.product_name;
                        }
                    }
                }

                if(em.updated_by !=null){
                    foreach(User user in users){
                        if(em.updated_by == user.user_id){
                            data.updated_name = user.full_name;
                        }
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<QrManagementDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/{qr_id}")]
        public async Task<ActionResult<QrManagementDto>> FetchQrDetails(Guid qr_id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify whether the plant had a key already
            var QrKey = _context.QrManagement.Find(qr_id);
            if(QrKey==null){
                return NotFound("Invalid qr_id");
            }

            //fetch the plant details
            var plant = _context.Plant.Find(QrKey.plant_id);
            //fetch the product details
            var product = new ProductManagement();
            if(QrKey.product_id!=null){
                product = _context.ProductManagement.Find(QrKey.product_id);
            }
            var user = new User();
            if(QrKey.updated_by!=null){
                //fetch the user details
                var users = _context.User.Where(x=> x.user_id == QrKey.updated_by).ToList();
                if(users.Count > 0){
                    user = users[0];
                }
            }

                       
            
            

            return Ok(new QrManagementDto{
                        qr_id = QrKey.qr_id,
                        product_id = QrKey.product_id,
                        plant_id = QrKey.plant_id,
                        public_id = QrKey.public_id,
                        manufactured_date = QrKey.manufactured_date,
                        expiry_date = QrKey.expiry_date,
                        product_mrp_copy = QrKey.product_mrp_copy,
                        pack_id = QrKey.pack_id,
                        serial_number = QrKey.serial_number,
                        batch_no = QrKey.batch_no,
                        status = QrKey.status.ToString(),
                        created_at_ip = QrKey.created_at_ip,
                        updated_by = QrKey.updated_by,
                        created_at = QrKey.created_at,
                        last_updated_at = QrKey.last_updated_at,
                        plant_name = plant.plant_name,
                        product_name = product.product_name,
                        updated_name = user.full_name
            });
            

        }


        [AllowAnonymous]
        [HttpGet("public/{qr_id}")]
        public async Task<ActionResult<ProductInfoDto>> FetchPublicQrDetails(String qr_id)
        {
            //decrypt the qr_id
            var utils = new Utils();
            var decrypted = utils.QrDecryption(qr_id);
            Console.WriteLine(decrypted);
            // verify whether the plant had a key already
            var QrKey =await _context.QrManagement.Where(x => x.public_id == decrypted).ToListAsync();
            if(QrKey.Count<1){
                return NotFound("Record not found");
            }
            else if(QrKey[0].status != QrManagementStatus.PRINTED){
                return NotFound("Record not found");
            }
            //fetch the plant details
            var plant = _context.Plant.Find(QrKey[0].plant_id);
            //fetch the product details
            var product = new ProductManagement();
            if(QrKey[0].product_id!=null){
                product = _context.ProductManagement.Find(QrKey[0].product_id);
            }
            //fetch the clients ip address           
            var ip_add = HttpContext.Connection.RemoteIpAddress.ToString();
            //create a record in QrReadActivity
            var read_record = new QrReadActivity{
                    qr_id = QrKey[0].qr_id,
                    product_id = QrKey[0].product_id ?? new Guid(),
                    ip_address = ip_add
                };

            var read_activity = _context.QrReadActivity.Add(read_record);
            if(!(await _context.SaveChangesAsync()>0)){
                return NotFound("Record not found");
            }
            return Ok(new ProductInfoDto{
                        public_id = QrKey[0].public_id,
                        manufactured_date = QrKey[0].manufactured_date,
                        expiry_date = QrKey[0].expiry_date,
                        product_mrp_copy = QrKey[0].product_mrp_copy,
                        pack_id = QrKey[0].pack_id,
                        serial_number = QrKey[0].serial_number,
                        batch_no = QrKey[0].batch_no,
                        plant_name = plant.plant_name,
                        product_name = product.product_name,
                        product_logo = product.product_logo,
                        product_writeup = product.product_writeup,
                        plant_location_geo = plant.plant_location_geo,
                        qr_id = QrKey[0].qr_id,
                        product_id = QrKey[0].product_id ?? new Guid(),
                        qr_read_activity_id = read_record.qr_read_activity_id
            });
            

        }
    }
}