using System.Security.Claims;
using API.DTOs;
using API.DTOs.Request;
using API.Middleware;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/request")]
    public class RequestController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public RequestController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [CustomAuthorization(AccessLevelsDto.PLANT_MANAGER)]
        [HttpPost("create/")]
        public async Task<ActionResult> Create(CreateRequestDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            //check : already requested or not
            var request = _context.RequestProductAccess.Where(x => x.product_id == managementDto.product_id & x.plant_id == managementDto.plant_id & x.status == RequestProductAccessStatus.PENDING).ToList();
            if(request.Count > 0){
                return NotFound("Already requested");
            }
            
            //check whether the product had linked to this plant or not
            var link = _context.ProductPlantMapping.Where(x=> x.product_id==managementDto.product_id & x.plant_id==managementDto.plant_id & x.status == PlantStatusOptions.ACTIVE).ToList();
            if(link.Count > 0){
                return NotFound("Already Mapped");
            }

            //check whether the plant_id is valid or not
            var plants = _context.Plant.Where(x => x.operated_id == logged_user.user_id).ToList();
            if(plants.Count<1){
                return NotFound("you are not a plant manager");
            }
            var plant = plants[0];


            //check whether the product_id is valid or not
            var product = _context.ProductManagement.Find(managementDto.product_id);
            if(product == null){
                return NotFound("invalid product_id");
            }


            //create the record in the db
            _context.RequestProductAccess.Add(
                new RequestProductAccess{
                    product_id = product.product_id ,
                    plant_id = plant.plant_id,
                    message = managementDto.message,
                    requested_qr_limit = managementDto.requested_qr_limit,
                    requested_by_user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to create the record");

            return Ok("Request raised successfully");

        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<CreateRequestDto>>> FetchAllRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.RequestProductAccess.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderBy(x=>x.status).OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).OrderByDescending(x => x.status).Skip(skip).Take(param.PageSize).ToList();
            }
            
            //fetch the user_ids to fetch their details
            List<Guid?> user_ids = new List<Guid?>();
            foreach (RequestProductAccess plt in email)
            {
                if(! user_ids.Contains(plt.requested_by_user_id)) user_ids.Add(plt.requested_by_user_id);
                
                if(plt.responded_by_user_id != null){
                   if(! user_ids.Contains(plt.responded_by_user_id)) user_ids.Add(plt.responded_by_user_id);
                }
            }
            //fetch the product_ids to get their details
            List<Guid?> product_ids = new List<Guid?>();
            foreach (RequestProductAccess plt in email)
            {
                if(!(product_ids.Contains(plt.product_id))) product_ids.Add(plt.product_id);
            }

            //fetch the plant_ids to get their details
            List<Guid?> plant_ids = new List<Guid?>();
            foreach (RequestProductAccess plt in email)
            {
                if(!(plant_ids.Contains(plt.plant_id))) plant_ids.Add(plt.plant_id);
            }


            var users = _context.User.Where(x=>user_ids.Contains(x.user_id)).ToList();
            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();
            var plants = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();

            List<CreateRequestDto> details_final = new List<CreateRequestDto>();
            foreach(RequestProductAccess em in email){
                var data=new CreateRequestDto{
                        request_product_access_id = em.request_product_access_id,
                        product_id = em.product_id,
                        plant_id = em.plant_id,
                        message = em.message,
                        requested_qr_limit = em.requested_qr_limit,
                        requested_by_user_id = em.requested_by_user_id,
                        responded_by_user_id = em.responded_by_user_id,
                        status = em.status.ToString(),
                        created_at = em.created_at,
                        last_updated_at = em.last_updated_at
                    };

                foreach(User user in users){
                    if(user.user_id == em.requested_by_user_id) data.requested_by_user_name = user.UserName;
                    if(em.responded_by_user_id!=null){
                        if(user.user_id == em.responded_by_user_id) data.responded_by_user_name = user.UserName;
                    }
                }

                foreach(ProductManagement product in products){
                    if(product.product_id == em.product_id) data.product_name = product.product_name;
                }

                foreach(Plant plant in plants){
                    if(plant.plant_id == em.plant_id){
                        data.plant_name = plant.plant_name;
                        data.plant_code = plant.plant_code;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<CreateRequestDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("respond/")]
        public async Task<ActionResult> Respond(RespondRequest dto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var request = _context.RequestProductAccess.Find(dto.request_product_access_id);
            if(request==null){
                return NotFound("Record not found");
            }
            var status_dict = new Dictionary<string, RequestProductAccessStatus>(){
                {"ACCEPT", RequestProductAccessStatus.ACCEPTED},
                {"REJECT", RequestProductAccessStatus.REJECTED},
            };

            if(request.status != RequestProductAccessStatus.PENDING){
                return NotFound("Already responded");
            }

            request.status = status_dict[dto.status];
            request.last_updated_at = DateTime.Now;

            if(status_dict[dto.status] == RequestProductAccessStatus.ACCEPTED){
                //check whether the product had linked to this plant or not
                var link = _context.ProductPlantMapping.Where(x=> x.product_id==request.product_id & x.plant_id==request.plant_id & x.status == PlantStatusOptions.ACTIVE).ToList();
                if(link.Count == 0){
                    //create the record in the db
                    _context.ProductPlantMapping.Add(
                        new ProductPlantMapping{
                            product_id = request.product_id,
                            plant_id = request.plant_id,
                            created_by = logged_user.user_id
                        }
                    );
                }
            }
            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to respond the record");

            return Ok("Responded successfully");

        }
    }
}