using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence;

namespace API.Controllers
{
    [CustomAuthorization(AccessLevelsDto.ADMIN)]
    [ApiController]
    [Route("api/[controller]")]
    public class PlantKeyController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public PlantKeyController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }
        

        [HttpPost("create/")]
        public async Task<ActionResult> Create(PlantKeyManagementDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify the plant status
            var plant = _context.Plant.Find(managementDto.plant_id);
            if(plant==null){
                return NotFound("Invalid plant_id");
            }else if(plant.status == PlantStatusOptions.INACTIVE){
                return NotFound("Plant is in Inactive State");
            }

            // verify whether the plant had a key already
            var plantKey = _context.PlantKeyManagement.Where(x => x.plant_id == managementDto.plant_id & x.status == PlantStatusOptions.ACTIVE).ToList();
            if(plantKey.Count >0){
                return NotFound("Plant already had an active key");
            }

            // create the record
            var newRecord = _context.PlantKeyManagement.Add(
                new PlantKeyManagement{
                    plant_id = managementDto.plant_id,
                    plant_key = managementDto.plant_key,
                    created_by = logged_user.user_id
                }
            );

            // var new_obj = new PlantKeyManagement{
            //     plant_key_id = newRecord.Entity.plant_key_id,
            //     plant_id = newRecord.Entity.plant_id,
            //     plant_key = newRecord.Entity.plant_key,
            //     status = newRecord.Entity.status,
            //     created_by=newRecord.Entity.created_by,
            //     created_at=newRecord.Entity.created_at,
            //     last_updated_at=newRecord.Entity.last_updated_at
            // }.ToString();

            // Console.WriteLine(new_obj);
            // // var obj = JsonSerializer.Serialize(newRecord.CurrentValues);

            // //create a tracker record
            // _context.TrackingPlantKeyManagement.Add(
            //     new TrackingPlantKeyManagement{
            //         new_obj = new_obj,
            //         plant_key_id = newRecord.Entity.plant_key_id,
            //         user_id = logged_user.user_id
            //     }
            // );
            

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("Key not created");
            }

            return Ok("Key created successfully");
            

        }

        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(PlantKeyManagementEditDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // verify whether the plant had a key already
            var plantKey = _context.PlantKeyManagement.Where(x => x.plant_key_id == managementDto.plant_key_id).ToList();
            if(plantKey.Count < 1){
                return NotFound("Invalid plant_key_id");
            }
            plantKey[0].plant_key = managementDto.plant_key ?? plantKey[0].plant_key;

            if(managementDto.status != null){
                var status = managementDto.status == "ACTIVE" ? PlantStatusOptions.ACTIVE : PlantStatusOptions.INACTIVE;
                plantKey[0].status = status;
            }
            plantKey[0].last_updated_at = DateTime.Now;           

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("Key not updated");
            }

            return Ok("Key updated successfully");
            

        }

        [HttpPost("get/all/")]
        public async Task<PagedResult<List<PlantKeyManagementEditDto>>> FetchSmtpRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.PlantKeyManagement.Where(x => x.status == PlantStatusOptions.ACTIVE).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid> plant_ids = new List<Guid>();
            foreach(PlantKeyManagement plt in email){
                plant_ids.Add(plt.plant_id);
            }

            // #fetch the Plants
            var plants = _context.Plant.Where(x=> plant_ids.Contains(x.plant_id)).ToList();


            List<PlantKeyManagementEditDto> details_final = new List<PlantKeyManagementEditDto>();
            foreach(PlantKeyManagement em in email){
                foreach(Plant plant in plants){
                    if(plant.plant_id==em.plant_id){
                        details_final.Add(
                            new PlantKeyManagementEditDto{
                                plant_key_id = em.plant_key_id,
                                plant_id = em.plant_id,
                                plant_name = plant.plant_name,
                                plant_key = em.plant_key,
                                status = em.status.ToString()
                            }
                        );                        
                    }
                }
               
            };

            return PagedResult<List<PlantKeyManagementEditDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }
    }
}