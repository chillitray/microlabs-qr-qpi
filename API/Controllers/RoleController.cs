using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class RoleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public RoleController(DataContext context, UserManager<User> userManager)
        {
            this._userManager = userManager;
            this._context = context;
        }

        [CustomAuthorization(AccessLevelsDto.ROLE)]
        [HttpPost("create/")]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            // Console.WriteLine("HELLO");
            // var access_level_ip = access_level;
            // Console.WriteLine(roleDto.access);
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var access_dict = new Dictionary<string, AccessLevelOptions>(){
                {"PLANT_MANAGER", AccessLevelOptions.PLANT_MANAGER},
                {"ADMIN", AccessLevelOptions.ADMIN},
            };

            _context.Role.Add(
                new Role{
                    role = roleDto.role,
                    access_level = access_dict[roleDto.access],
                    created_by = logged_user.user_id
                }
            );

            if(!(await _context.SaveChangesAsync()>0)){
                return NotFound("Not created");
            }
            return Ok("Role Created Successfully");
        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("edit/")]
        public async Task<IActionResult> Edit(RoleDto role)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var role_db = _context.Role.Find(role.role_id);
            if(role_db ==null) return NotFound("Invalid role_id");
            role_db.role = role.role;
            role_db.last_updated_at = DateTime.Now; 

            if(!(await _context.SaveChangesAsync()>0)){
                return NotFound("Not updated");
            }
            return Ok("Role updated Successfully");
        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("delete/")]
        public async Task<IActionResult> Delete(RoleDto role)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var role_db = _context.Role.Find(role.role_id);
            if(role_db ==null) return NotFound("Invalid role_id");
            role_db.status = StatusOptions.INACTIVE;
            role_db.last_updated_at = DateTime.Now; 

            if(!(await _context.SaveChangesAsync()>0)){
                return NotFound("Not deleted");
            }
            return Ok("Role Deleted Successfully");
        }
        

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<RoleDto>>> FetchRoles([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.Role.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }


            List<RoleDto> details_final = new List<RoleDto>();
            foreach(Role em in email){           

                details_final.Add(new RoleDto{
                        role_id = em.role_id,
                        role = em.role,
                        access = em.access_level.ToString(),
                        created_at = em.created_at,
                        status = em.status.ToString(),
                    });
            };

            return PagedResult<List<RoleDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }
    }
}