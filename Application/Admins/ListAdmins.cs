using Application.Core;
using Application.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Admins
{
    public class ListAdmins
    {
        public class Query : IRequest<PagedResult<List<FetchAdminsDto>>> {
            public PagingParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedResult<List<FetchAdminsDto>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            { 
                this._context = context;
                
            }

            public async Task<PagedResult<List<FetchAdminsDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                  
                var skip = (request.Params.pageNumber - 1) * request.Params.PageSize ;

                //  fetch all the users from the db
                var users_x = await _context.User.Where(x => x.user_id != null).ToListAsync();

                // get the count of users
                var users_count = users_x.Count();

                // slice the records based on the pagination
                var users = users_x.OrderBy(x => x.joined_date).Skip(skip).Take(request.Params.PageSize).ToList();
                if(request.Params.Sort){
                    //sort in Descending order
                    if(request.Params.sortOnField=="full_name"){
                        //sort on full_name field
                        users = users_x.OrderByDescending( x => x.full_name).Skip(skip).Take(request.Params.PageSize).ToList();
                    }{
                        //sort on joined_date field
                        users = users_x.OrderByDescending( x => x.joined_date).Skip(skip).Take(request.Params.PageSize).ToList();
                    }

                }else{
                    // sort in Ascending order
                    if(request.Params.sortOnField=="full_name"){
                        //sort on full_name field
                        users = users_x.OrderBy( x => x.full_name).ToList();
                    }{
                        //sort on joined_date field
                        users = users_x.OrderBy( x => x.joined_date).ToList();
                    }

                }

                // fetch the role ids from the users
                List<Guid?> role_ids = new List<Guid?>();
                foreach(User plt in users){
                    role_ids.Add(plt.role_id);                    
                }

                //fetch the user_ids from the users
                List<Guid?> user_ids = new List<Guid?>();
                foreach(User plt in users){
                    role_ids.Add(plt.user_id);                    
                }

                // fetch the Role records wrt role_ids
                var roles = _context.Role.Where(x => role_ids.Contains(x.role_id)).Select(
                    x => new RoleDetailsDto(
                        x.role_id,
                        x.role,
                        x.access_level == AccessLevelOptions.ADMIN ? "ADMIN" : "PLANT_MANAGER"
                    )).ToList();

                //fetch the user last_login_date_time
                var login = _context.SessionActivity.Where(x => user_ids.Contains(x.user_id)).OrderBy(z => z.last_login).GroupBy(y=>y.user_id).ToList();

                List<FetchAdminsDto> admins_details = new List<FetchAdminsDto>();
                foreach(User user in users)
                {
                    var user_status = user.status == Domain.USerStatusOptions.ACTIVE ? "ACTIVE" : user.status == Domain.USerStatusOptions.BLOCKED ? "BLOCKED" : "VERIFICATION_PENDING";
                    var data = new FetchAdminsDto{
                        user_id = user.user_id,
                        email = user.Email,
                        emp_id = user.emp_id,
                        full_name = user.full_name,
                        status = user_status,
                        joined_date = user.joined_date,
                        last_login_date = null
                    };
                    foreach(RoleDetailsDto role in roles){
                        if(user.role_id == role.role_id){
                            data.role_details = role;
                            break;
                        }
                    }

                    foreach(SessionActivity log in login){
                        if(user.user_id == log.user_id){
                            data.last_login_date = log.last_login;
                        }
                    }
                    admins_details.Add(data);
                }

                

                return PagedResult<List<FetchAdminsDto>>.Success(admins_details,request.Params.pageNumber,request.Params.PageSize,users_count);
            }
        }
    }
}