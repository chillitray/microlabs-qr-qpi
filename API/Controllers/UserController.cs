
using API.DTOs;
using API.Middleware;
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
    [Route("api/user_information/")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public UserController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [AllowAnonymous]
        [HttpPost("create/")]
        public async Task<ActionResult> Create(CreateUserInfoDto dto)
        {

            if(dto.qr_read_activity_id != null){
                //check: record is already created or not
                var record = _context.UserInformation.Where(x => x.qr_read_activity_id == dto.qr_read_activity_id).ToList();
                if(record.Count>0){
                    return Ok("Added successfully");
                }
            }
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            //create the record in the db
            _context.UserInformation.Add(
                new UserInformation{
                    full_name = dto.full_name,
                    email = dto.email,
                    phone_no = dto.phone_no,
                    country_code = dto.country_code,
                    location_address = dto.location_address,
                    location_city = dto.location_city,
                    location_state = dto.location_state,
                    location_country = dto.location_country,
                    location_pincode = dto.location_pincode,
                    product_id = dto.product_id ?? new Guid(),
                    qr_read_activity_id = dto.qr_read_activity_id,
                    qr_id = dto.qr_id ?? new Guid(),
                    ip_address = ip 

                }
            );

            var result = await _context.SaveChangesAsync()>0;
            if(!result){
                return NotFound("Internal Error");
            }

            return Ok("Added successfully");
        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<CreateUserInfoDto>>> FetchAll([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.UserInformation.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }
            
            //fetch the product_ids to get their details
            List<Guid?> product_ids = new List<Guid?>();
            foreach (UserInformation plt in email)
            {
                if(! product_ids.Contains(plt.product_id)) product_ids.Add(plt.product_id);
            }

            //fetch the qr_ids to get their details
            List<Guid?> qr_ids = new List<Guid?>();
            foreach (UserInformation plt in email)
            {
                if(! qr_ids.Contains(plt.qr_id)) qr_ids.Add(plt.qr_id);
            }

            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();
            var qrs = _context.QrManagement.Where(x=>qr_ids.Contains(x.qr_id)).ToList();

            List<CreateUserInfoDto> details_final = new List<CreateUserInfoDto>();
            foreach(UserInformation em in email){
                var data=new CreateUserInfoDto{
                        full_name = em.full_name,
                        product_id = em.product_id,
                        created_at = em.created_at,
                        email = em.email,
                        phone_no = em.phone_no,
                        country_code = em.country_code,
                        location_address = em.location_address,
                        location_city = em.location_city,
                        location_state = em.location_state,
                        location_country = em.location_country,
                        location_pincode = em.location_pincode,
                        qr_id = em.qr_id,
                        ip_address = em.ip_address
                    };


                foreach(ProductManagement product in products){
                    if(product.product_id == em.product_id) data.product_name = product.product_name;
                }

                foreach(QrManagement qr in qrs){
                    if(qr.qr_id == em.qr_id) data.qr_public_id = qr.public_id;
                }

                

                

                details_final.Add(data);
            };

            return PagedResult<List<CreateUserInfoDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/{id}/")]
        public async Task<PagedResult<List<CreateUserInfoDto>>> FetchProductSpecific(Guid id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.UserInformation.Where(x => x.product_id == id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }
            
            //fetch the product_ids to get their details
            List<Guid?> product_ids = new List<Guid?>();
            foreach (UserInformation plt in email)
            {
                if(! product_ids.Contains(plt.product_id)) product_ids.Add(plt.product_id);
            }

            //fetch the qr_ids to get their details
            List<Guid?> qr_ids = new List<Guid?>();
            foreach (UserInformation plt in email)
            {
                if(! qr_ids.Contains(plt.qr_id)) qr_ids.Add(plt.qr_id);
            }

            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();
            var qrs = _context.QrManagement.Where(x=>qr_ids.Contains(x.qr_id)).ToList();

            List<CreateUserInfoDto> details_final = new List<CreateUserInfoDto>();
            foreach(UserInformation em in email){
                var data=new CreateUserInfoDto{
                        full_name = em.full_name,
                        product_id = em.product_id,
                        created_at = em.created_at,
                        email = em.email,
                        phone_no = em.phone_no,
                        country_code = em.country_code,
                        location_address = em.location_address,
                        location_city = em.location_city,
                        location_state = em.location_state,
                        location_country = em.location_country,
                        location_pincode = em.location_pincode,
                        qr_id = em.qr_id,
                        ip_address = em.ip_address
                    };


                foreach(ProductManagement product in products){
                    if(product.product_id == em.product_id) data.product_name = product.product_name;
                }

                foreach(QrManagement qr in qrs){
                    if(qr.qr_id == em.qr_id) data.qr_public_id = qr.public_id;
                }

                

                

                details_final.Add(data);
            };

            return PagedResult<List<CreateUserInfoDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }
    }
}