using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [CustomAuthorization(AccessLevelsDto.ADMIN)]
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


        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(QrManagementFetchDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify whether the plant had a key already
            var QrKey = _context.QrManagement.Find(managementDto.qr_id);
            if(QrKey==null){
                return NotFound("Invalid qr_id");
            }

            var type_dict = new Dictionary<string, QrManagementStatus>(){
                {"PRINTED", QrManagementStatus.PRINTED},
                {"BLOCKED", QrManagementStatus.BLOCKED},
                {"COUNTERFEIT_BLOCKED", QrManagementStatus.COUNTERFEIT_BLOCKED}
            };

            QrKey.status = type_dict[managementDto.status];
            QrKey.updated_by = logged_user.user_id;
            QrKey.last_updated_at = DateTime.Now;

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("Status not updated");
            }

            return Ok("Status updated successfully");
            

        }

        [HttpPost("get/all/")]
        public async Task<PagedResult<List<QrManagementDto>>> FetchSmtpRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email =await _context.QrManagement.Where(x => true).ToListAsync();
            var count = email.Count;

            //sort the records in descending order and fetch the product ids
            email = email.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
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
    }
}