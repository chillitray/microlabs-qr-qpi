

using System.Reflection;
using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using API.Services;
using API.Trackers;
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
    [Route("api/counterfiet/")]
    public class CounterfietController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public CounterfietController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [HttpPost("create/")]
        public async Task<ActionResult> Create(CounterfietDto dto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));


            var type_dict = new Dictionary<string, CounterfietTypeOptions>(){
                {"IP_ADDRESS", CounterfietTypeOptions.IP_ADDRESS},
                {"LOCATION", CounterfietTypeOptions.LOCATION},
                {"NUMBER_OF_SCANS", CounterfietTypeOptions.NUMBER_OF_SCANS}
            };

            // check whether we already have record or not
            var counterfiets = _context.CounterfietManagement.Where(x=>x.product_id==dto.product_id & x.counterfeit_type==type_dict[dto.counterfeit_type]).ToList();
            if(counterfiets.Count>0){
                return NotFound("Already have a record on this type");
            }

            //verify the product_id
            var product = _context.ProductManagement.Find(dto.product_id);
            if(product==null){
                return NotFound("invalid product_id");
            }
            //create the record in the db
            var newRecord = _context.CounterfietManagement.Add(
                new CounterfietManagement{
                    product_id = dto.product_id ?? new Guid(),
                    counterfeit_type = type_dict[dto.counterfeit_type],
                    low_risk_threshold = dto.low_risk_threshold,
                    moderate_threshold = dto.moderate_threshold,
                    high_risk_threshold = dto.high_risk_threshold,
                    created_by = logged_user.user_id
                }
            );

            //format the data to string
            var new_obj_string = new TrackerUtils().CreateCounterfietObj(newRecord.Entity);
            
           _context.TrackingCounterfeitManagement.Add(
                new TrackingCounterfeitManagement{
                    new_obj = new_obj_string,
                    counterfeit_mgmt_id = newRecord.Entity.counterfeit_mgmt_id,
                    user_id = logged_user.user_id
                }
            );
            

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to create the record");

            return Ok("Record created successfully");

        }
    


        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(CounterfietDto dto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));


            var type_dict = new Dictionary<string, CounterfietTypeOptions>(){
                {"IP_ADDRESS", CounterfietTypeOptions.IP_ADDRESS},
                {"LOCATION", CounterfietTypeOptions.LOCATION},
                {"NUMBER_OF_SCANS", CounterfietTypeOptions.NUMBER_OF_SCANS}
            };

            // check whether we already have record or not
            var counterfiets = _context.CounterfietManagement.Where(x=>x.product_id==dto.product_id & x.counterfeit_type==type_dict[dto.counterfeit_type]).ToList();
            if(counterfiets.Count<1){
                return NotFound("Record not found");
            }

            var counterfiet = counterfiets[0];
            //format the data to string
            var old_obj_string = new TrackerUtils().CreateCounterfietObj(counterfiet);

            counterfiet.low_risk_threshold = dto.low_risk_threshold;
            counterfiet.moderate_threshold = dto.moderate_threshold;
            counterfiet.high_risk_threshold = dto.high_risk_threshold;
            counterfiet.last_updated_at = DateTime.Now;

            
            //format the data to string
            var new_obj_string = new TrackerUtils().CreateCounterfietObj(counterfiet);


            //add record in tracker
            _context.TrackingCounterfeitManagement.Add(
                new TrackingCounterfeitManagement{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    counterfeit_mgmt_id =counterfiet.counterfeit_mgmt_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to edit the record");

            return Ok("Record edited successfully");

        }


        [HttpPost("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));


            // check whether we already have record or not
            var counterfiets = _context.CounterfietManagement.Where(x=>x.product_id==id).ToList();
            if(counterfiets.Count<1){
                return NotFound("Records not found");
            }

            

            //delete the record
            foreach(CounterfietManagement counter in counterfiets){
                //format the data to string
                var old_obj_string = new TrackerUtils().CreateCounterfietObj(counter);
                _context.CounterfietManagement.Remove(counter);

                //add record in tracker
                _context.TrackingCounterfeitManagement.Add(
                    new TrackingCounterfeitManagement{
                        old_obj = old_obj_string,
                        new_obj = "{}",
                        counterfeit_mgmt_id =counter.counterfeit_mgmt_id,
                        user_id = logged_user.user_id
                    }
                );

            }

            

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to delete the records");

            return Ok("Record deleted successfully");

        }


        [HttpPost("get/all/")]
        public async Task<PagedResult<List<CounterfietDto>>> FetchAllRecords([FromQuery]PagingParams param)
        {
            Console.WriteLine("Hello");
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.CounterfietManagement.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }
            
            //fetch the product_ids to get their details
            List<Guid?> product_ids = new List<Guid?>();
            foreach (CounterfietManagement plt in email)
            {
                if(!(product_ids.Contains(plt.product_id))) product_ids.Add(plt.product_id);
            }


            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();

            List<CounterfietDto> details_final = new List<CounterfietDto>();
            foreach(CounterfietManagement em in email){
                var data=new CounterfietDto{
                        counterfeit_mgmt_id = em.counterfeit_mgmt_id,
                        product_id = em.product_id,
                        counterfeit_type = em.counterfeit_type.ToString(),
                        low_risk_threshold = em.low_risk_threshold,
                        moderate_threshold = em.moderate_threshold,
                        high_risk_threshold = em.high_risk_threshold,
                        created_at = em.created_at,
                        last_updated_at = em.last_updated_at
                    };


                foreach(ProductManagement product in products){
                    if(product.product_id == em.product_id) data.product_name = product.product_name;
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<CounterfietDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("get/{id}")]
        public async Task<Result<List<CounterfietDto>>> GetProductCounterfiets(Guid id)
        {
            var counterfiets =await _context.CounterfietManagement.Where(x=>x.product_id==id).ToListAsync();

            List<CounterfietDto> details_final = new List<CounterfietDto>();
            var product = _context.ProductManagement.Find(id);
            if(product==null){
                return Result<List<CounterfietDto>>.Success(details_final);
            }

            
            foreach(CounterfietManagement em in counterfiets){
                details_final.Add(new CounterfietDto{
                        counterfeit_mgmt_id = em.counterfeit_mgmt_id,
                        product_id = em.product_id,
                        counterfeit_type = em.counterfeit_type.ToString(),
                        low_risk_threshold = em.low_risk_threshold,
                        moderate_threshold = em.moderate_threshold,
                        high_risk_threshold = em.high_risk_threshold,
                        created_at = em.created_at,
                        last_updated_at = em.last_updated_at,
                        product_name = product.product_name
                    });

            }
            return Result<List<CounterfietDto>>.Success(details_final);
        }
    }

}