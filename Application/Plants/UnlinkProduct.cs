using Application.Core;
using Application.Trackers;
using Domain;
using MediatR;
using Persistence;

namespace Application.Plants
{
    public class UnlinkProduct
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid PlantId { get; set; }

            public Guid ProductId { get; set; }
            public User logged_user {get; set;}
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                this._context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var logged_user = request.logged_user;
                // fetch the db object
                var activity = _context.ProductPlantMapping.Where(x => x.plant_id==request.PlantId & x.product_id == request.ProductId ).ToList();
                
                
                if(activity.Count<1) return null;
                //format the data to string
                var old_obj_string2 = new TrackerUtils().CreateProductPlantmappingObj(activity[0]);
                try{

                    if(activity[0].status==Domain.PlantStatusOptions.ACTIVE){
                        // make the mapping inactive
                        activity[0].status=Domain.PlantStatusOptions.INACTIVE;
                    }else{
                        // make the mapping active
                        activity[0].status=Domain.PlantStatusOptions.ACTIVE;
                    }

                    
                    activity[0].last_updated_at = DateTime.Now;
                }catch (Exception ex)
                {
                    return Result<Unit>.Failure(ex.Message);
                }
                
                //format the data to string
                var new_obj_string2 = new TrackerUtils().CreateProductPlantmappingObj(activity[0]);
                _context.TrackingProductPlantMapActivity.Add(
                    new TrackingProductPlantMapActivity{
                        old_obj = old_obj_string2,
                        new_obj = new_obj_string2,
                        product_plant_mapping_id = activity[0].product_plant_mapping_id,
                        user_id = logged_user.user_id
                    }
                );


                //  save the chnages to the db
                var result = await _context.SaveChangesAsync() > 0;

                if(!result) return Result<Unit>.Failure("Failed to deactivate the Product-Plant mapping");
                
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}