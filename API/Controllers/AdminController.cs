using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using API.Services;
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
            var result = await Mediator.Send(new ListAdmins.Query { Params = param });
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
            Console.WriteLine("username: "+username);
            //generarte a random password
            var utils = new Utils();
            var password = utils.RandomPassword();
            Console.WriteLine("password: "+password);

            //check given role_id is valid or not
            var role = _context.Role.Find(details.role_id);
            if (role == null) return NotFound("invalid role_id");
            if (role.status == StatusOptions.INACTIVE) return NotFound("role is in inactive state");

            var plant = new Plant();
            if (details.plant_id != null)
            {
                //check for plant_id is valid or not
                plant = _context.Plant.Find(details.plant_id);
                if (plant == null) return NotFound("invalid plant_id");
                //check if plant is active or not
                if (plant.status != PlantStatusOptions.ACTIVE) return NotFound("plant is in inactive state");
            }

            var new_users = new List<User>{
                    new User
                    {
                        emp_id = details.emp_id,
                        full_name = details.full_name,
                        role_id = details.role_id,
                        plant_id = details.plant_id,
                        created_by = logged_user.user_id,
                        //IdentityUser Mandatory fields
                        Email = details.Email,
                        UserName = username
                    }
                };

            foreach(User user in new_users){
                    Console.WriteLine("Hello234");
                    var result = await _userManager.CreateAsync(user, "Pa$$w0rd");
                    Console.WriteLine(result);
                    if (!result.Succeeded) return NotFound("unable to add user. please try again");
            }
            

            //send a mail to the user
            var emailSubject = "MicroLabs";
            var emailBody = "You are added to the Microlabs portal. To SignIn use the following credentials email: "+ details.Email +" and password: "+password;

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
            if(users.Count <1){
                return NotFound("Invalid user_id");
            }
            var user = users[0];
            user.plant_id= details.plant_id ?? user.plant_id;
            user.full_name = details.full_name ?? user.full_name;
            user.role_id = details.role_id ?? user.role_id;
            user.last_updated_at = DateTime.Now;

            var result =await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to edit the details");

            return Ok("User details edited successfully");
        }


        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpPost("block/")]
        public async Task<IActionResult> ChangeAdminStatus(EditAdminDto details)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            //fetch the use details from the db
            var users = _context.User.Where(x => x.user_id == details.user_id).ToList();
            if(users.Count < 1){
                return NotFound("Invalid user_id");
            }
            var user = users[0];

            var status_dict = new Dictionary<string, USerStatusOptions>(){
                {"ACTIVE", USerStatusOptions.ACTIVE},
                {"BLOCKED", USerStatusOptions.BLOCKED},
            };
            user.status = status_dict[details.status];
            user.last_updated_at = DateTime.Now;
            

            var result =await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to update the user status");

            return Ok("User status updated successfully");
        }
    }
}