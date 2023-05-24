using System.Net;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;

namespace API.Middleware
{
    // [AttributeUsage(AttributeTargets.Class)]
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        // public Domain.AccessLevelOptions AccessLevel  { get; set; } = Domain.AccessLevelOptions.PLANT_MANAGER;
        // private readonly DataContext _context;
        private readonly AccessLevelsDto _AccessLevel;
        public CustomAuthorization( AccessLevelsDto AccessLevel)
        {
            this._AccessLevel = AccessLevel;
            // this._context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            

            var db = context.HttpContext.RequestServices.GetRequiredService<DataContext>();
            var ip_add =  context.HttpContext.Connection.RemoteIpAddress.ToString();

            // throw new NotImplementedException();
            Microsoft.Extensions.Primitives.StringValues Authorization;
            Microsoft.Extensions.Primitives.StringValues UserId;
            context.HttpContext.Request.Headers.TryGetValue("Authorization", out Authorization);
            context.HttpContext.Request.Headers.TryGetValue("UserId", out UserId);

            var _token = Authorization.FirstOrDefault();
            Guid _userId =new Guid(UserId.FirstOrDefault());
            
            if(_AccessLevel.ToString() == "ROLE" & _token == null){
                var users = db.User.Where(x=>true).Count();
                // Console.WriteLine("UsersCount:" + users);
                if(users<1){
                    // if no users present on the db then we can allow anonymus to create role 
                    return;
                }
            }
            

            if (_token != null)
            {
                Guid userId = _userId;
                string authToken = _token;
                if (authToken != null)  
                {
                    if (IsValidToken(authToken,userId,db, ip_add))  
                    {  
                        context.HttpContext.Response.Headers.Add("authToken", authToken);  
                        context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");  
  
                        context.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");      
                        return;  
                    }  
                    else  
                    {  
                        context.HttpContext.Response.Headers.Add("authToken", authToken);  
                        context.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");              
  
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;  
                        context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";  
                        context.Result = new JsonResult("NotAuthorized")  
                        {  
                            Value = new  
                            {  
                                Status = "Error",  
                                Message = "Invalid Token"  
                            },  
                        };  
                    }
                }

            }  
                
        }

        private bool IsValidToken(string authToken,Guid UserId,DataContext _context,String ip_add)
        {
            //remove the word Bearer from the token
            // Console.WriteLine(authToken);
            authToken = authToken.Remove(0,7);
            // Console.WriteLine(authToken);
            //check whther the session is expired or not
            var inactive = _context.InactiveSessionActivity.Where(x => x.user_access_token == authToken & x.user_id == UserId).ToList();
            if(inactive.Count > 0){
                // Console.WriteLine(1);
                return false;
            }
            // find the session in the db
            var sessions = _context.SessionActivity.Where(x => x.user_access_token == authToken & x.user_id == UserId).ToList();
            if(sessions.Count < 1){
                // Console.WriteLine(2);
                return false;
            }
            
            var session = sessions[0];
            // find whther the user is active or not
            var user = _context.User.Where(x => x.user_id == UserId & x.status != Domain.USerStatusOptions.BLOCKED).ToList();
            if(user.Count < 1){
                // Console.WriteLine(3);
                return false;
            }

            // find the access_level of the user
            var role = _context.Role.Find(user[0].role_id);
            if(role == null){
                // Console.WriteLine(4);
                return false;
            }
            if(_AccessLevel.ToString() == "ROLE" & role.access_level != AccessLevelOptions.ADMIN) return false;
            else if(_AccessLevel.ToString() != "ROLE" & _AccessLevel.ToString() != "BOTH" & role.access_level.ToString() != _AccessLevel.ToString() ){
                // Console.WriteLine(5);
                return false;
            }

            // #update the ip_addresss and last_access time in db
            sessions[0].last_access = DateTime.Now;
            sessions[0].last_access_ip = ip_add;
            
            var result2 =  _context.SaveChanges()>0;
            if(!result2){
                return false;
            }
            
            return true;
        }
    }
}