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
    public class PlantSessionController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public PlantSessionController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }

        [HttpPost("get/{plant_id}")]
        public async Task<PagedResult<List<PlantSessionFetchDto>>> FetchSmtpRecords(Guid plant_id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.PlantSessionManagement.Where(x => x.plant_id==plant_id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<PlantSessionFetchDto> details_final = new List<PlantSessionFetchDto>();
            foreach(PlantSessionManagement em in email){
                details_final.Add(
                    new PlantSessionFetchDto{
                        plant_session_id = em.plant_session_id,
                        plant_id = em.plant_id,
                        plant_key = em.plant_key,
                        plant_access_token = em.plant_access_token,
                        status = em.status.ToString(),
                        created_at = em.created_at,
                        expired_at = em.expired_at,
                        last_access = em.last_access,
                        last_access_ip = em.last_access_ip
                    }
                );
            };

            return PagedResult<List<PlantSessionFetchDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("edit/{plant_session_id}")]
        public async Task<ActionResult> Edit(Guid plant_session_id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify whether the plant had a key already
            var plantKey = _context.PlantSessionManagement.Find(plant_session_id);
            if(plantKey==null){
                return NotFound("Invalid plant_session_id");
            }

            plantKey.status = PlantSessionManagementStatus.EXPIRED;
           

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("Status not updated");
            }

            return Ok("Status updated successfully");
            

        }

    }
}