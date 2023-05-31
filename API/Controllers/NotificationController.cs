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
    [ApiController]
    [Route("api/notification/")]
    public class NotificationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public NotificationController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;

        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("create/")]
        public async Task<ActionResult> Create(CreateNotificationDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            //verify given notification_type is valid or not
            var noti_type = _context.NotificationTypeManagement.Find(managementDto.notification_type);
            if (noti_type == null)
            {
                return NotFound("inavlid notification_type");
            }
            if(managementDto.notification_type == null){
                return NotFound("notification_type is required");
            }

            //create the record in the db
            var newRecord =_context.Notification.Add(
                new Notification
                {
                    notification_content = managementDto.notification_content,
                    notification_type = managementDto.notification_type ?? new Guid(),
                    redirect_url = managementDto.redirect_url,
                    created_by = logged_user.user_id
                }
            );

            //format the data to string
            var new_obj_string = new TrackerUtils().CreateNotificationObj(newRecord.Entity);
            _context.TrackingNotification.Add(
                new TrackingNotification{
                    new_obj = new_obj_string,
                    notification_id = newRecord.Entity.notification_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return NotFound("Unable to create the record");

            return Ok("Record created successfully");

        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(CreateNotificationDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var noti = await _context.Notification.FindAsync(managementDto.notification_id);
            if (noti == null)
            {
                return NotFound("Record not found");
            }

            //format the data to string
            var old_obj_string = new TrackerUtils().CreateNotificationObj(noti);

            noti.notification_content = managementDto.notification_content ?? noti.notification_content;
            noti.redirect_url = managementDto.redirect_url ?? noti.redirect_url;
            if (managementDto.notification_type != null)
            {
                //verify given notification_type is valid or not
                var noti_type = _context.NotificationTypeManagement.Find(managementDto.notification_type);
                if (noti_type == null)
                {
                    return NotFound("inavlid notification_type");
                }
                noti.notification_type = managementDto.notification_type ?? new Guid();
            }

            if (managementDto.status != null)
            {
                noti.status = managementDto.status == "ACTIVE" ? PlantStatusOptions.ACTIVE : PlantStatusOptions.INACTIVE;
            }

            noti.last_updated_at = DateTime.Now;
            //format the data to string
            var new_obj_string = new TrackerUtils().CreateNotificationObj(noti);
            _context.TrackingNotification.Add(
                new TrackingNotification{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    notification_id = noti.notification_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return NotFound("Unable to edit the record");

            return Ok("Record edited successfully");

        }

        
        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("get/all/")]
        public async Task<PagedResult<List<CreateNotificationDto>>> FetchAllRecords([FromQuery] PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize;

            var email_x = await _context.Notification.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order 
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if (!param.Sort)
            {
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            //fetch the notification_type_ids to fetch related records from notification_type
            List<Guid?> notification_type_ids = new List<Guid?>();
            foreach (Notification plt in email)
            {
                notification_type_ids.Add(plt.notification_type);
            }
            var noti_types = _context.NotificationTypeManagement.Where(x => notification_type_ids.Contains(x.notification_type_id)).ToList();

            List<CreateNotificationDto> details_final = new List<CreateNotificationDto>();
            foreach (Notification em in email)
            {
                var data = new CreateNotificationDto
                {
                    notification_id = em.notification_id,
                    notification_content = em.notification_content,
                    notification_type = em.notification_type,
                    redirect_url = em.redirect_url,
                    status = em.status.ToString(),
                    created_at = em.created_at
                };

                foreach(NotificationTypeManagement type in noti_types){
                    if(type.notification_type_id==em.notification_type){
                        data.notification_type_type = type.notifiication_type;
                        data.notification_priority = type.priority.ToString();
                    }
                }

                details_final.Add(data);
            };

            return PagedResult<List<CreateNotificationDto>>.Success(details_final, param.pageNumber, param.PageSize, count);

        }

        [CustomAuthorization(AccessLevelsDto.BOTH)]
        [HttpPost("get/user/")]
        public async Task<PagedResult<List<FetchUserNotificationDto>>> FetchUserRecords(UserDto user_dto , [FromQuery] PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize;

            var email_x = await _context.NotificationActivity.Where(x => x.user_id==user_dto.UserId).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order 
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if (!param.Sort)
            {
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            //fetch the notification_ids to fetch its related records
            List<Guid?> notification_ids = new List<Guid?>();
            foreach (NotificationActivity plt in email)
            {
                notification_ids.Add(plt.notification_id);
            }

            var notifications = _context.Notification.Where(x=>notification_ids.Contains(x.notification_id)).ToList();

            //fetch the notification_type_ids to fetch its related records
            List<Guid?> notification_type_ids = new List<Guid?>();
            foreach (Notification plt in notifications)
            {
                notification_type_ids.Add(plt.notification_type);
            }
            var noti_types = _context.NotificationTypeManagement.Where(x => notification_type_ids.Contains(x.notification_type_id)).ToList();

            List<FetchUserNotificationDto> details_final = new List<FetchUserNotificationDto>();
            foreach (NotificationActivity em in email)
            {
                var data = new FetchUserNotificationDto
                {
                    notification_activity_id = em.notification_activity_id,                    
                    notification_id = em.notification_id,
                    user_id = em.user_id,
                    first_read_at = em.first_read_at,
                    read_count = em.read_count,
                    created_at = em.created_at
                };
                
                foreach(Notification noti in notifications){
                    if(noti.notification_id == em.notification_id){
                        foreach(NotificationTypeManagement type in noti_types){
                            if(type.notification_type_id==noti.notification_type){
                                data.notification_type_type = type.notifiication_type;
                                data.notification_priority = type.priority.ToString();
                                data.notification_content = noti.notification_content;
                                data.redirect_url = noti.redirect_url;
                                data.notification_type = noti.notification_type;
                            }
                        }   
                    }

                }

                

                details_final.Add(data);
            };

            return PagedResult<List<FetchUserNotificationDto>>.Success(details_final, param.pageNumber, param.PageSize, count);

        }




        [CustomAuthorization(AccessLevelsDto.BOTH)]
        [HttpPost("read/")]
        public async Task<ActionResult> Read(ReadNotificationDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var noti = await _context.NotificationActivity.FindAsync(managementDto.notification_activity_id);
            if (noti == null)
            {
                return NotFound("Record not found");
            }
            else if(noti.user_id != logged_user.user_id){
                return NotFound("Record not found");
            }

            if(noti.first_read_at == null){
                noti.first_read_at = DateTime.Now;
            }
            noti.read_count +=1;
            noti.last_updated_at = DateTime.Now;

            // #save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return NotFound("Internal error");

            return Ok("notification read successfully");

        }
            
    }
}