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
    [Route("api/rate_limits/")]
    
    public class RateLimitsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public RateLimitsController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [HttpPost("create/")]
        public async Task<ActionResult> Create(RateLimitDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var type_dict = new Dictionary<string, RateTypeOptions>(){
                {"PARTICULAR_IP", RateTypeOptions.PARTICULAR_IP},
                {"PARTICULAR_USER_ID", RateTypeOptions.PARTICULAR_USER_ID},
                {"MAX_LOGIN_FAILED_ATTEMPTS", RateTypeOptions.MAX_LOGIN_FAILED_ATTEMPTS}
            };
            var rate_type = type_dict[managementDto.rate_type];

            //verify whether the record is already created
            var rate_limits =await _context.RateLimits.Where(x => x.rate_type == rate_type & x.status == PlantStatusOptions.ACTIVE ).ToListAsync();
            // if any records exists make them inactive
            foreach(RateLimits rate in rate_limits){
                rate.status = PlantStatusOptions.INACTIVE;
                rate.last_updated_at = DateTime.Now;
            }

            //create the record in the db
            _context.RateLimits.Add(
                new RateLimits{
                    rate_type = type_dict[managementDto.rate_type],
                    max_allowed_per_day = managementDto.max_allowed_per_day,
                    max_allowed_overall = managementDto.max_allowed_overall,
                    created_by = logged_user.user_id
                }
            );

            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = "",
                    message = "Added rate limmit record",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to create the record");

            return Ok("Record created successfully");

        }


        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(RateLimitDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var rate_limit =await _context.RateLimits.FindAsync(managementDto.rate_limit_id);
            if(rate_limit==null){
                return NotFound("Record not found");
            }

            rate_limit.max_allowed_overall = managementDto.max_allowed_overall ?? rate_limit.max_allowed_overall;
            if(managementDto.max_allowed_per_day != 0){
                rate_limit.max_allowed_per_day = managementDto.max_allowed_per_day;
            }
            rate_limit.last_updated_at = DateTime.Now;
            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = "",
                    message = "Edited rate limmit record",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );
            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to edit the record");

            return Ok("Record edited successfully");

        }


        [HttpPost("delete/{rate_limit_id}")]
        public async Task<ActionResult> Delete(Guid rate_limit_id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var rate_limit =await _context.RateLimits.FindAsync(rate_limit_id);
            if(rate_limit==null){
                return NotFound("Record not found");
            }
            if(rate_limit.status == PlantStatusOptions.INACTIVE){
                rate_limit.status = PlantStatusOptions.ACTIVE;
            }else{
                rate_limit.status = PlantStatusOptions.INACTIVE;
            }
            
            rate_limit.last_updated_at = DateTime.Now;
            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = "",
                    message = "Removed rate limmit record",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to edit the record");

            return Ok("Record edited successfully");

        }

        [HttpPost("get/all/")]
        public async Task<PagedResult<List<RateLimitDto>>> FetchSmtpRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.RateLimits.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }


            List<RateLimitDto> details_final = new List<RateLimitDto>();
            foreach(RateLimits em in email){
                var data=new RateLimitDto{
                        rate_limit_id = em.rate_limit_id,
                        rate_type = em.rate_type.ToString(),
                        max_allowed_per_day = em.max_allowed_per_day,
                        max_allowed_overall = em.max_allowed_overall,
                        status = em.status.ToString(),
                        created_at = em.created_at,
                    };

                

                details_final.Add(data);
            };

            return PagedResult<List<RateLimitDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


    }
}