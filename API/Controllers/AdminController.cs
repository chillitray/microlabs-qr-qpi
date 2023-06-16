using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using API.Services;
using API.Trackers;
using Application.Admins;
using Application.Core;
using Application.DTOs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly IMediator _meditor;
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public AdminController(IMediator mediator, DataContext context, UserManager<User> userManager, IEmailService emailService)
        {
            this._emailService = emailService;
            this._userManager = userManager;
            this._context = context;
            this._meditor = mediator;
        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpGet]
        public async Task<IActionResult> GetAdmins([FromQuery] PagingParams param) //[FromQuery]PagingParams param
        {
            //  this api fetches all the admins

            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var result = await Mediator.Send(new ListAdmins.Query { Params = param, logged_user = logged_user });
            if (result == null) return NotFound();
            return Ok(PagedResult<List<FetchAdminsDto>>.Success(result.Value, result.PageNumber, result.PageSize, result.TotalRecords));
        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("employee/create/")]
        public async Task<IActionResult> CreateAdmin(AddAdminDto details)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var users = await _context.User.Where(x => true).ToListAsync();

            //check whether the emp_id is unique or not
            var check_users = users.Where(x => x.emp_id == details.emp_id).ToList();
            if (check_users.Count > 0) return NotFound("emp_id is already taken");

            //check email_id_  is unique or not
            check_users = users.Where(x => x.Email == details.Email).ToList();
            if (check_users.Count > 0) return NotFound("email is already taken");

            //since we are using Identity package for Authentication purpose username is mandatory in User table
            // so generating the random username
            var username = Guid.NewGuid().ToString("N");
            // Console.WriteLine("username: "+username);
            //generarte a random password
            var utils = new Utils();
            var password = utils.RandomPassword();
            // Console.WriteLine("password: "+password);

            //check given role_id is valid or not
            var role = _context.Role.Find(details.role_id);
            if (role == null) return NotFound("invalid role_id");
            if (role.status == StatusOptions.INACTIVE) return NotFound("role is in inactive state");

            // var plant = new Plant();
            // if (details.plant_id != null)
            // {
            //     //check for plant_id is valid or not
            //     plant = _context.Plant.Find(details.plant_id);
            //     if (plant == null) return NotFound("invalid plant_id");
            //     //check if plant is active or not
            //     if (plant.status != PlantStatusOptions.ACTIVE) return NotFound("plant is in inactive state");
            // }

            var new_users = new List<User>{
                new User
                    {
                        Email = details.Email,
                        emp_id = details.emp_id,
                        full_name = details.full_name,
                        role_id = details.role_id,
                        // plant_id = details.plant_id,
                        UserName = username,
                        created_by = logged_user.user_id
                    }
                };

            foreach (User user_record in new_users)
            {
                // Console.WriteLine("Hello234");
                var result = await _userManager.CreateAsync(user_record, password);
                // Console.WriteLine("Hello234");
                // Console.WriteLine(result);
                if (!result.Succeeded) return NotFound("unable to add user. please try again");
            }

            var new_user_db = new AddUserTrackerDto
            {
                user_id = new_users[0].user_id,
                Email = details.Email,
                emp_id = details.emp_id,
                full_name = details.full_name,
                role_id = details.role_id,
                // plant_id = details.plant_id,
                created_by = logged_user.user_id,
                joined_date = new_users[0].joined_date,
                last_updated_at= new_users[0].last_updated_at,
                status = new_users[0].status.ToString(),
                created_at = new_users[0].created_at
            };
            // //format the data to string
            var new_obj_string = new TrackerUtils().CreateUserbj(new_user_db);
            _context.TrackingUserEditActivity.Add(
                new TrackingUserEditActivity{
                    new_obj = new_obj_string,
                    user_id = new_users[0].user_id,
                    edited_by = logged_user.user_id
                }
            );

            //send a mail to the user
            var emailSubject = "MicroLabs";
            var emailBody = "You are added to the Microlabs portal. To SignIn use the following credentials email: " + details.Email + " and password: " + password;

            var emailData = new EmailData
            {
                EmailToId = details.Email,
                EmailToName = details.full_name,
                EmailSubject = emailSubject,
                EmailBody = emailBody
            };

            var status = _emailService.SendEmail(emailData, 1);
            // add record in trackerEmail
            var option = TrackerEmailStatus.GENERATED;
            if (status)
            {
                option = TrackerEmailStatus.SENT;
            }
            else
            {
                option = TrackerEmailStatus.NOT_DELIVERED;
            }
            _context.TrackerEmail.Add(
                new TrackerEmail
                {
                    email = details.Email,
                    email_subject = emailSubject,
                    email_body = emailBody,
                    reason = TrackerEmailReason.SIGN_UP,
                    status = option,
                    user_id = new_users[0].user_id
                }
            );

            if (option == TrackerEmailStatus.NOT_DELIVERED)
            {
                return NotFound("User created but Unable to send Email");
            }

            var message = "Added new Plant Manager";
            if(role.access_level == AccessLevelOptions.ADMIN){
                message = "Added new Admin";
            }
            _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = new_obj_string,
                        message = message,
                        severity_type = SeverityType.SEMI_CRITICAL,
                        user_id = logged_user.user_id
                    }
                );

            _context.SaveChanges();

            return Ok("User added to the portal");
        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("edit/")]
        public async Task<IActionResult> EditAdmin(EditAdminDto details)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            //fetch the use details from the db
            var users = _context.User.Where(x => x.user_id == details.user_id).ToList();
            if (users.Count < 1)
            {
                return NotFound("Invalid user_id");
            }
            var user = users[0];
            if(details.role_id!=null){
                //verify the role_id
                var role = _context.Role.Find(details.role_id);
                if (role == null) return NotFound("invalid role_id");
                if (role.status == StatusOptions.INACTIVE) return NotFound("role is in inactive state");
            }
            // if(details.plant_id!=null){
            //     //verify the role_id
            //     var role = _context.Plant.Find(details.plant_id);
            //     if (role == null) return NotFound("invalid plant_id");
            //     if (role.status == PlantStatusOptions.INACTIVE) return NotFound("Plant is in inactive state");
            // }


            // //format the data to string
            var old_user_db = new AddUserTrackerDto
            {
                user_id = user.user_id,
                Email = user.Email,
                emp_id = user.emp_id,
                full_name = user.full_name,
                role_id =user.role_id,
                // plant_id = user.plant_id,
                created_by = user.created_by,
                joined_date = user.joined_date,
                last_updated_at= user.last_updated_at,
                status = user.status.ToString(),
                created_at = user.created_at
            };
            var old_obj_string = new TrackerUtils().CreateUserbj(old_user_db);

            // user.plant_id = details.plant_id ?? user.plant_id;
            user.full_name = details.full_name ?? user.full_name;
            user.role_id = details.role_id ?? user.role_id;
            user.last_updated_at = DateTime.Now;
            
            // TODO : need to take care of product logo

            // //format the data to string
            var new_user_db = new AddUserTrackerDto
            {
                user_id = user.user_id,
                Email = user.Email,
                emp_id = user.emp_id,
                full_name = user.full_name,
                role_id =user.role_id,
                // plant_id = user.plant_id,
                created_by = user.created_by,
                joined_date = user.joined_date,
                last_updated_at= user.last_updated_at,
                status = user.status.ToString(),
                created_at = user.created_at
            };
            var new_obj_string = new TrackerUtils().CreateUserbj(new_user_db);
            _context.TrackingUserEditActivity.Add(
                new TrackingUserEditActivity{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    user_id = user.user_id,
                    edited_by = logged_user.user_id
                }
            );

            var role_db = _context.Role.Find(user.role_id);
            var message = "Edited Plant manager details";
            if(role_db.access_level == AccessLevelOptions.ADMIN){
                message = "Edited Admin details";
            }

            _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = new_obj_string,
                        message = message,
                        severity_type = SeverityType.SEMI_CRITICAL,
                        user_id = logged_user.user_id
                    }
            );

            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return NotFound("Unable to edit the details");

            return Ok("User details edited successfully");
        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("block/")]
        public async Task<IActionResult> ChangeAdminStatus(EditAdminDto details)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            //fetch the use details from the db
            var users = _context.User.Where(x => x.user_id == details.user_id).ToList();
            if (users.Count < 1)
            {
                return NotFound("Invalid user_id");
            }
            var user = users[0];
            //format the data to string
            var old_user_db = new AddUserTrackerDto
            {
                user_id = user.user_id,
                Email = user.Email,
                emp_id = user.emp_id,
                full_name = user.full_name,
                role_id =user.role_id,
                // plant_id = user.plant_id,
                created_by = user.created_by,
                joined_date = user.joined_date,
                last_updated_at= user.last_updated_at,
                status = user.status.ToString(),
                created_at = user.created_at
            };
            var old_obj_string = new TrackerUtils().CreateUserbj(old_user_db);


            var status_dict = new Dictionary<string, USerStatusOptions>(){
                {"ACTIVE", USerStatusOptions.ACTIVE},
                {"BLOCKED", USerStatusOptions.BLOCKED},
            };
            user.status = status_dict[details.status];
            user.last_updated_at = DateTime.Now;

            
            var new_user_db = new AddUserTrackerDto
            {
                user_id = user.user_id,
                Email = user.Email,
                emp_id = user.emp_id,
                full_name = user.full_name,
                role_id =user.role_id,
                // plant_id = user.plant_id,
                created_by = user.created_by,
                joined_date = user.joined_date,
                last_updated_at= user.last_updated_at,
                status = user.status.ToString(),
                created_at = user.created_at
            };
            var new_obj_string = new TrackerUtils().CreateUserbj(new_user_db);
            _context.TrackingUserEditActivity.Add(
                new TrackingUserEditActivity{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    user_id = user.user_id,
                    edited_by = logged_user.user_id
                }
            );

            var role_db = _context.Role.Find(user.role_id);
            var message = "Plant manager ";
            if(role_db.access_level == AccessLevelOptions.ADMIN){
                message = "Admin ";
            }
            if(details.status == "ACTIVE"){
                message = "Unblocked "+message;
            }else{
                message = "Blocked "+message;
            }
            _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = new_obj_string,
                        message = message,
                        severity_type = SeverityType.SEMI_CRITICAL,
                        user_id = logged_user.user_id
                    }
            );


            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return NotFound("Unable to update the user status");

            return Ok("User status updated successfully");
        }
    }
}