using Application.Core;
using Application.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Plants
{
    public class List
    {
        public class Query : IRequest<PagedResult<List<FetchPlantsDto>>> {
            public Guid Id { get; set; }
            public PagingParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedResult<List<FetchPlantsDto>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            { 
                this._context = context;
                
            }

            public async Task<PagedResult<List<FetchPlantsDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // var activities =  
                var skip = (request.Params.pageNumber - 1) * request.Params.PageSize ;
                // var plant_ids_count = _context.ProductPlantMapping.Where(x => (x.product_id == request.Id) & (x.status==PlantStatusOptions.ACTIVE)).Count();
                

                var plants =await _context.ProductPlantMapping.Where(x => (x.product_id == request.Id) & (x.status==PlantStatusOptions.ACTIVE)).ToListAsync();
                // fetch the total no of records
                var plant_ids_count = plants.Count();

                //sort the records in descending order and fetch the product ids
                var plant_ids = plants.OrderByDescending(x=> x.created_at).Select(n => n.plant_id).Skip(skip).Take(request.Params.PageSize).ToList();

                // Console.WriteLine(request.Params.Sort);
                if(!request.Params.Sort){
                    // Console.WriteLine(request.Params.Sort);
                    plant_ids = plants.OrderBy(x=> x.created_at).Select(n => n.plant_id).Skip(skip).Take(request.Params.PageSize).ToList();
                    // Console.WriteLine(plant_ids[0]);
                }
                // Console.WriteLine(plant_ids[0]);
                
                var plant_details = _context.Plant.Where(x => plant_ids.Contains(x.plant_id)).OrderBy(x => x.plant_name).ToList();
                
                List<Guid?> admins_ids = new List<Guid?>();
                foreach(Plant plt in plant_details){

                    if (plt.operated_id != null){
                        admins_ids.Add(plt.operated_id);
                    }
                    
                }
                // Console.WriteLine(admins_ids.Count);
                
                // Console.WriteLine(plant_details.Count);
                var user_details =_context.User.Where(x => admins_ids.Contains(x.user_id)).ToList();
                // Console.WriteLine(user_details.Count);

                //fetch the plant key
                var plantKey = _context.PlantKeyManagement.Where(x => x.status == PlantStatusOptions.ACTIVE).ToList();

                List<FetchPlantsDto> plant_details_final = new List<FetchPlantsDto>();
                // Console.WriteLine("hello");
                foreach(Plant plt in plant_details){
                    // Console.WriteLine("loop1");
                    var data = new FetchPlantsDto{
                                    plant_name=plt.plant_name,
                                    plant_code = plt.plant_code,
                                    plant_location_address = plt.plant_location_address,
                                    // plant_location_city=plt.plant_location_city,
                                    // plant_location_state=plt.plant_location_state,
                                    // plant_location_country=plt.plant_location_country,
                                    plant_qr_limit=plt.plant_qr_limit,
                                    // plant_status=plt.status,
                                    plant_id=plt.plant_id                                    
                                };
                    foreach(User usr in user_details){

                        // Console.WriteLine(usr.user_id);
                        // Console.WriteLine(plt.operated_id);
                        if(usr.user_id == plt.operated_id){

                            data.admin_user_id=usr.user_id;
                            data.admin_name=usr.full_name;
                            data.admin_emp_id=usr.emp_id;
                            data.admin_email=usr.Email;
                            
                        }
                    }

                    data.plant_key_status = "INACTIVE";
                    foreach(PlantKeyManagement key in plantKey){
                        if(key.plant_id == plt.plant_id){
                            data.plant_key = key.plant_key;
                            data.plant_key_status = key.status.ToString();
                        }
                    }


                    plant_details_final.Add(data);
                }

                // var skip = (request.Params.pageNumber - 1) * request.Params.PageSize ;
                // var output = plant_details_final.Skip(skip).Take(request.Params.PageSize).ToList();

                // return Result<PagedList<FetchPlantsDto>>.Success(
                //     new PagedList<FetchPlantsDto>(plant_details_final, plant_ids_count,request.Params.pageNumber, request.Params.PageSize)
                // );

                return PagedResult<List<FetchPlantsDto>>.Success(plant_details_final,request.Params.pageNumber,request.Params.PageSize,plant_ids_count);
            }
        }

    }
}