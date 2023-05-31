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
    [Route("api/[controller]")]
    public class SmtpController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public SmtpController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
        }


        [HttpPost("create/")]
        public async Task<ActionResult> CreateSmtp(SmtpDto smtp)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));


            var email_type_dict = new Dictionary<string, EmailTypeOptions>(){
                {"NOTIFICATION", EmailTypeOptions.NOTIFICATION},
                {"REPLY", EmailTypeOptions.REPLY},
                {"AUTHENTICATION", EmailTypeOptions.AUTHENTICATION}
            };

            var email_type = email_type_dict[smtp.email_type];
            

            //delete if there were any previous records
            var email = _context.SmtpConfig.Where(x => x.email_type == email_type & x.status == PlantStatusOptions.ACTIVE).ToList();
        
            foreach(SmtpConfig em in email){
                // _context.SmtpConfig.Remove(em);

                em.status = PlantStatusOptions.INACTIVE;
            }
            
            // create a record in db
            var newRecord = _context.SmtpConfig.Add(new SmtpConfig{
                max_emails_per_day = smtp.max_emails_per_day,
                email_id = smtp.email_id,
                password = smtp.password,
                email_type = email_type,
                created_by = logged_user.user_id
            });


            //format the data to string
            var new_obj_string = new TrackerUtils().CreateSmtpObj(newRecord.Entity);
            _context.TrackingEditSmtpConfig.Add(
                new TrackingEditSmtpConfig{
                    new_obj = new_obj_string,
                    smtp_config_id = newRecord.Entity.smtp_config_id,
                    user_id = logged_user.user_id
                }
            );

            //save the changes
            var result = await _context.SaveChangesAsync() > 0;
            if(!result){
                return NotFound("SMTP record not created");
            }

            return Ok("SMTP record created successfully");
            

        }


        [HttpPost("get/all/")]
        public async Task<Result<List<SmtpDto>>> FetchSmtpRecords()
        {
            var email =await _context.SmtpConfig.Where(x => x.status == PlantStatusOptions.ACTIVE).ToListAsync();

            List<SmtpDto> email_details_final = new List<SmtpDto>();
            foreach(SmtpConfig em in email){
                email_details_final.Add(
                    new SmtpDto{
                        smtp_config_id = em.smtp_config_id,
                        email_id = em.email_id,
                        password = em.password,
                        max_emails_per_day = em.max_emails_per_day,
                        email_type = em.email_type.ToString()
                    }
                );
            };

            return Result<List<SmtpDto>>.Success(email_details_final);

        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteSmtpRecord(Guid id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var email =await _context.SmtpConfig.FindAsync(id);
            if(email==null){
                return NotFound("Invalid smtp_config_id");
            }

            //format the data to string
            var old_obj_string = new TrackerUtils().CreateSmtpObj(email);

            if(email.status == PlantStatusOptions.ACTIVE){
                email.status = PlantStatusOptions.INACTIVE;
            }
            email.last_updated_at = DateTime.Now;

            //format the data to string
            var new_obj_string = new TrackerUtils().CreateSmtpObj(email);

            //add record in tracker
            _context.TrackingEditSmtpConfig.Add(
                new TrackingEditSmtpConfig{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    smtp_config_id = email.smtp_config_id,
                    user_id = logged_user.user_id
                }
            );


            var result = await _context.SaveChangesAsync()>0;
            if(!result){
                return NotFound("Not Deleted");
            }

            return Ok("Deleted Successfully");

        }




        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditSmtpRecord(Guid id,SmtpDto smtp)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            var email =await _context.SmtpConfig.FindAsync(id);
            if(email==null){
                return NotFound("Invalid smtp_config_id");
            }

            //format the data to string
            var old_obj_string = new TrackerUtils().CreateSmtpObj(email);

            var email_type = smtp.email_type ?? email.email_type.ToString();
            var email_type_dict = new Dictionary<string, EmailTypeOptions>(){
                {"NOTIFICATION", EmailTypeOptions.NOTIFICATION},
                {"REPLY", EmailTypeOptions.REPLY},
                {"AUTHENTICATION", EmailTypeOptions.AUTHENTICATION}
            };

            var type = email_type_dict[email_type];

            email.password = smtp.password ?? email.password;
            if(smtp.max_emails_per_day != 0){
                email.max_emails_per_day = smtp.max_emails_per_day;
            }            
            email.email_type = type;
            email.last_updated_at = DateTime.Now;

            //format the data to string
            var new_obj_string = new TrackerUtils().CreateSmtpObj(email);

            //add record in tracker
            _context.TrackingEditSmtpConfig.Add(
                new TrackingEditSmtpConfig{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    smtp_config_id = email.smtp_config_id,
                    user_id = logged_user.user_id
                }
            );

            var result = await _context.SaveChangesAsync()>0;
            if(!result){
                return NotFound("Not Deleted");
            }

            return Ok("Edited Successfully");

        }


    }
}