using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using API.Middleware;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly DataContext _context;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<User> userManager, TokenService tokenService, DataContext context, IEmailService emailService)
        {
            this._emailService = emailService;
            this._context = context;
            this._tokenService = tokenService;
            this._userManager = userManager;

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(loginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return NotFound("Invalid Email");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (result)
            {
                if (loginDto.Otp == null)
                {
                    var db_otps_all = _context.TrackerOtp.Where(x => x.user_id == user.user_id & x.reason == TrackerOtpReason.SIGN_IN).ToList();
                    // delete previous otps: if any
                    var db_otps = db_otps_all.Where(x => x.status == TrackerOtpStatus.GENERATED).ToList();
                    foreach (TrackerOtp db_otp in db_otps)
                    {
                        db_otp.status = TrackerOtpStatus.FAILED;
                    }

                    // Fetch today's failed login attempts count
                    var count = db_otps_all.Where(x => x.status == TrackerOtpStatus.FAILED & x.created_at >= DateTime.Today).ToList();
                    // check whether the user exceeded todays login failed attempts
                    var rate_limits = _context.RateLimits.Where(x => x.rate_type == RateTypeOptions.MAX_LOGIN_FAILED_ATTEMPTS & x.status==PlantStatusOptions.ACTIVE).ToList();
                    if(rate_limits.Count<1){
                        return NotFound("Rate limit records not found");
                    }
                    var rate_record = rate_limits[0];
                    if (count.Count >= rate_record.max_allowed_per_day)
                    {
                        return NotFound("Rate limit Exceeded. Please try again Tomorrow");
                    }

                    // #send the otp
                    var utils = new Utils();
                    string otp = utils.GenerateRandomOTP();
                    var emailSubject = "MicroLabs - OTP";
                    var emailBody = "Otp to Login is " + otp;

                    var emailData = new EmailData
                    {
                        EmailToId = user.Email,
                        EmailToName = user.full_name,
                        EmailSubject = emailSubject,
                        EmailBody = emailBody
                    };

                    var status = _emailService.SendEmail(emailData,1);
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
                            email = user.Email,
                            email_subject = emailSubject,
                            email_body = emailBody,
                            reason = TrackerEmailReason.SIGN_IN,
                            status = option,
                            user_id = user.user_id
                        }
                    );

                    if (option == TrackerEmailStatus.NOT_DELIVERED)
                    {
                        await _context.SaveChangesAsync();
                        return NotFound("Unable to send Email");
                    }

                    // insert the otp in the db
                    _context.TrackerOtp.Add(
                        new TrackerOtp
                        {
                            email = user.Email,
                            user_id = user.user_id,
                            otp = otp
                        }
                    );

                    var result5 = await _context.SaveChangesAsync() > 0;
                    if (!result5)
                    {
                        return NotFound(("Failed to Login"));
                    }

                    return Ok("OTP Sent!");

                }

                // verfiy the otp
                var otp_db = _context.TrackerOtp.Where(x => x.user_id == user.user_id & x.reason == TrackerOtpReason.SIGN_IN & x.status == TrackerOtpStatus.GENERATED).ToList();
                if (otp_db.Count < 1)
                {
                    // # no active otps
                    return NotFound("OTP Expired");
                }
                else if (otp_db[0].created_at <= DateTime.Now.AddMinutes(-3))
                {
                    otp_db[0].status = TrackerOtpStatus.FAILED;
                    await _context.SaveChangesAsync();
                    return NotFound("OTP Expired");
                }
                else if (otp_db[0].otp != loginDto.Otp)
                {
                    //otp entered incorrectly
                    otp_db[0].failed_attempts += 1;
                    otp_db[0].last_attempted_at = DateTime.Now;

                    if (otp_db[0].failed_attempts >= 5)
                    {
                        // otp had entered incorrectly for 5 times
                        otp_db[0].status = TrackerOtpStatus.FAILED;
                        await _context.SaveChangesAsync();
                        return NotFound("OTP Expired");
                    }
                    await _context.SaveChangesAsync();
                    return NotFound("Incorrect OTP");
                }


                var token = _tokenService.CreateToken(user);
                var ip_add = HttpContext.Connection.RemoteIpAddress.ToString();
                // Console.WriteLine("Hello - IP");
                // Console.WriteLine(ip_add);
                // Console.WriteLine( HttpContext.Connection.LocalIpAddress);

                //remove previous sessions from the db
                var sessions = _context.SessionActivity.Where(x => x.user_id == user.user_id).ToList();

                foreach (SessionActivity se in sessions)
                {
                    _context.SessionActivity.Remove(se);
                    // #create record in inactive Session
                    _context.InactiveSessionActivity.Add(
                        new InactiveSessionActivity
                        {
                            session_id = se.session_id,
                            user_id = se.user_id,
                            user_access_token = se.user_access_token,
                            last_login = se.last_login,
                            last_login_ip = se.last_login_ip,
                            last_access_ip = se.last_access_ip,
                            last_access = se.last_access,
                            status = InactiveSessionStatusOptions.AUTO_LOGGED_OUT
                        }
                    );

                }

                // #create a record in SessionActivity Table
                var session = _context.SessionActivity.Add(
                    new SessionActivity
                    {
                        user_id = user.user_id,
                        user_access_token = token,
                        last_login_ip = ip_add,
                        last_access_ip = ip_add
                    }

                );

                // change the otp status to verified
                otp_db[0].status = TrackerOtpStatus.VERIFIED;
                user.status = USerStatusOptions.ACTIVE;

                _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj="",
                        message = "LogIn",
                        severity_type = SeverityType.NON_CRITICAL,
                        user_id = user.user_id
                    }
            );

                var result2 = await _context.SaveChangesAsync() > 0;
                if (!result2)
                {
                    return NotFound(("Failed to Login"));
                }


                return new UserDto
                {
                    UserId = user.user_id,
                    Token = token
                };
            }

            return NotFound("Wrong Email and Password combination");

        }

        [CustomAuthorization(AccessLevelsDto.BOTH)]
        [HttpPost("logout")]

        public async Task<ActionResult> Logout()
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            await _userManager.UpdateSecurityStampAsync(logged_user);

            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            // migrate the active session to inactive Session
            var sessions = _context.SessionActivity.Where(x => x.user_id == logged_user.user_id & x.user_access_token == _bearer_token).ToList();

            foreach (SessionActivity se in sessions)
            {
                _context.SessionActivity.Remove(se);
                // #create record in inactive Session
                _context.InactiveSessionActivity.Add(
                    new InactiveSessionActivity
                    {
                        session_id = se.session_id,
                        user_id = se.user_id,
                        user_access_token = se.user_access_token,
                        last_login = se.last_login,
                        last_login_ip = se.last_login_ip,
                        last_access_ip = se.last_access_ip,
                        last_access = se.last_access,
                        status = InactiveSessionStatusOptions.LOGGEDOUT
                    }
                );

            }

            _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj="",
                        message = "Logout",
                        severity_type = SeverityType.NON_CRITICAL,
                        user_id = logged_user.user_id
                    }
            );


            var result2 = await _context.SaveChangesAsync() > 0;
            if (!result2)
            {
                return NotFound(("Failed to LogOut"));
            }

            return Ok();

        }


        [AllowAnonymous]
        [HttpPost("password/forgot/")]
        public async Task<ActionResult> ForgotPassword(loginDto login)
        {
            // var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            var logged_user = await _userManager.FindByEmailAsync(login.Email);

            if (logged_user == null) return NotFound("User not found");

            // var token = await _userManager.GeneratePasswordResetTokenAsync(logged_user);
            var utils = new Utils();
            string token = utils.GenerateRandomOTP(2);

            // var link = Url.Action(
            //     action: "account", controller: "password/reset/", values:token="token", protocol: "http", host: "localhost:5000"
            // );
            
            // var link = Url.Action(nameof(AccountController), "password/reset/", new { token, email = logged_user.Email }, Request.Scheme);
            string link = Request.Scheme + "://" + Request.Host + "/api/account/password/verify/?token="+token;
            // Console.WriteLine(link);
            var emailSubject = "MicroLabs - Forgot Password";
            var emailBody = "Click on the link to reset the password " + link;

            var emailData = new EmailData
            {
                EmailToId = logged_user.Email,
                EmailToName = logged_user.full_name,
                EmailSubject = emailSubject,
                EmailBody = emailBody
            };

            var status = _emailService.SendEmail(emailData,1);
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
                    email = logged_user.Email,
                    email_subject = emailSubject,
                    email_body = emailBody,
                    reason = TrackerEmailReason.OTP_FORGOT_PASSWORD_VERIFY,
                    status = option,
                    user_id = logged_user.user_id
                }
            );

            if (option == TrackerEmailStatus.NOT_DELIVERED)
            {
                await _context.SaveChangesAsync();
                return NotFound("Unable to send Email");
            }

            // insert the otp in the db
            _context.TrackerOtp.Add(
                new TrackerOtp
                {
                    email = logged_user.Email,
                    user_id = logged_user.user_id,
                    otp = token,
                    reason = TrackerOtpReason.FORGOT_PASSWORD
                }
            );

            var result5 = await _context.SaveChangesAsync() > 0;
            if (!result5)
            {
                return NotFound(("Failed to Reset password"));
            }

            return Ok("Reset the password using the link sent via Email");


        }

        [AllowAnonymous]
        [HttpPost("password/verify")]
        public async Task<ActionResult> ResetPassword(String token, loginDto login)
        {
            // Console.WriteLine(token);

            var tracker_otp = await _context.TrackerOtp.Where(x=>x.otp == token & x.status == TrackerOtpStatus.GENERATED & x.reason== TrackerOtpReason.FORGOT_PASSWORD).ToListAsync();
            if(tracker_otp.Count < 1){
                return NotFound("Invalid Token");
            }

            var otp_db = tracker_otp[0];

            var user = _context.User.Where(x => x.user_id == otp_db.user_id).ToList();
            if(user.Count <1){
                return NotFound("Invalid Token");
            }
            var user_db = user[0];

            var result = await _userManager.RemovePasswordAsync(user_db);
            var result2 = await _userManager.AddPasswordAsync(user_db,login.Password);

            // Console.WriteLine(result2);
            otp_db.status = TrackerOtpStatus.VERIFIED;
            await _context.SaveChangesAsync();

            
            return Ok("Password reset successfully");
        }


        [CustomAuthorization(AccessLevelsDto.BOTH)]
        [HttpPost("password/change/")]

        public async Task<ActionResult> ChangePassword(ChangePasswordDto change)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if(change.oldPassword == change.newPassword){
                return NotFound("newPassword should be different from oldPasssword");
            }

            var password = await _userManager.ChangePasswordAsync(logged_user,change.oldPassword,change.newPassword);

            // Console.WriteLine(password);
            // var result2 = await _userManager.SaveChangesAsync() > 0;
            if (!password.Succeeded)
            {
                return NotFound("Failed to Change password");
            }

            _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = "{}",
                        message = "Changed Password",
                        severity_type = SeverityType.NON_CRITICAL,
                        user_id = logged_user.user_id
                    }
            );

            _context.SaveChanges();
            return Ok();

        }

        [AllowAnonymous]        
        [HttpPost("test/")]
        public async Task<ActionResult<List<String>>> TestApi()
        {
            // var users = await _context.User.Where(x=>true).ToListAsync();
            // var user = users[0].ToString();
            var notification = await _context.NotificationActivity.FirstAsync();
            Dictionary<String,String>dict = new Dictionary<string, string>();
            dict["user"]="hello";
            dict["hi"]="hello2";

            var utils = new Utils();

            var stringvers = utils.DictionaryToString(dict);
            // Console.WriteLine(stringvers.GetType());

            var result = stringvers.Split(',').ToDictionary(e=>e.Split(':')[0], e=>e.Split(':')[1]);
            // var utils = new Utils();
            // Console.WriteLine(result.GetType());
            List<String> numbers = new List<String>();

            // var num ="0000000000";
            // while(true){
            //     num = utils.GenerateNextString(num);
            //     numbers.Add(num);
            //     if(numbers.Count() >= 100){
            //         break;
            //     }
            // }
            // _context.NotificationActivity.Add(
            //     new NotificationActivity{
            //         notification_id = new Guid("ead6b428-5668-4a69-add7-08db5b9e5fe0"),
            //         user_id = new Guid("beedde76-8dc8-4889-a65a-cb6c6f785e9f")                    
            //     }
            // );
            // _context.NotificationActivity.Add(
            //     new NotificationActivity{
            //         notification_id = new Guid("ead6b428-5668-4a69-add7-08db5b9e5fe0"),
            //         user_id = new Guid("beedde76-8dc8-4889-a65a-cb6c6f785e9f")                    
            //     }
            // );
            // _context.NotificationActivity.Add(
            //     new NotificationActivity{
            //         notification_id = new Guid("ead6b428-5668-4a69-add7-08db5b9e5fe0"),
            //         user_id = new Guid("1093e3a1-8170-4004-9385-1994f6715536")                    
            //     }
            // );
            // _context.SaveChanges();

            return Ok(numbers);
        }
        

    }
}