using System.Security.Claims;
using API.DTOs;
using API.Middleware;
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
    [Route("api/notification_type_mgmt/")]

    public class NotificationTypeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public NotificationTypeController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [HttpPost("create/")]
        public async Task<ActionResult> Create(NotificationTypeDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var type_dict = new Dictionary<string, NotificationPriority>(){
                {"LOW", NotificationPriority.LOW},
                {"MODERATE", NotificationPriority.MODERATE},
                {"HIGH", NotificationPriority.HIGH}
            };
            

            //create the record in the db
            var newRecord = _context.NotificationTypeManagement.Add(
                new NotificationTypeManagement{
                    notifiication_type = managementDto.notifiication_type,
                    priority = type_dict[managementDto.priority],
                    notification_for = managementDto.notification_for,
                    created_by = logged_user.user_id
                }
            );

            //format the data to string
            var new_obj_string = new TrackerUtils().CreateNotificationTypeObj(newRecord.Entity);
            _context.TrackingNotificationManagement.Add(
                new TrackingNotificationManagement{
                    new_obj = new_obj_string,
                    notification_type_id = newRecord.Entity.notification_type_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to create the record");

            return Ok("Record created successfully");

        }

        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(NotificationTypeDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var noti =await _context.NotificationTypeManagement.FindAsync(managementDto.notification_type_id);
            if(noti==null){
                return NotFound("Record not found");
            }

            //format the data to string
            var old_obj_string = new TrackerUtils().CreateNotificationTypeObj(noti);

            noti.notifiication_type = managementDto.notifiication_type ?? noti.notifiication_type;
            if(managementDto.priority != null){
                var type_dict = new Dictionary<string, NotificationPriority>(){
                    {"LOW", NotificationPriority.LOW},
                    {"MODERATE", NotificationPriority.MODERATE},
                    {"HIGH", NotificationPriority.HIGH}
                };
                noti.priority = type_dict[managementDto.priority];
            }
            noti.notification_for = managementDto.notification_for ?? noti.notification_for;

            if(managementDto.status != null){
                noti.status = managementDto.status == "ACTIVE" ? PlantStatusOptions.ACTIVE : PlantStatusOptions.INACTIVE; 
            }

            noti.last_updated_at = DateTime.Now;
            //format the data to string
            var new_obj_string = new TrackerUtils().CreateNotificationTypeObj(noti);
            _context.TrackingNotificationManagement.Add(
                new TrackingNotificationManagement{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    notification_type_id = noti.notification_type_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to edit the record");

            return Ok("Record edited successfully");

        }

        
        
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<NotificationTypeDto>>> FetchAllRecords([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.NotificationTypeManagement.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }


            List<NotificationTypeDto> details_final = new List<NotificationTypeDto>();
            foreach(NotificationTypeManagement em in email){
                var data=new NotificationTypeDto{
                        notification_type_id = em.notification_type_id,
                        notifiication_type = em.notifiication_type,
                        priority = em.priority.ToString(),
                        notification_for = em.notification_for,
                        status = em.status.ToString(),
                        created_at = em.created_at
                    };

                

                details_final.Add(data);
            };

            return PagedResult<List<NotificationTypeDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }
        
    }
}