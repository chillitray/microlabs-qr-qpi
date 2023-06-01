using Application.Core;
using Application.Trackers;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Plants
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid PlantId { get; set; }
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
                // var activity = _context.ProductPlantMapping.Where(x => x.plant_id==request.PlantId & x.product_id == request.ProductId).ToList();
                
                // Console.WriteLine("Hello");
                // if(activity.Count<1) return null;
                // try{
                //     activity[0].status=Domain.PlantStatusOptions.INACTIVE;
                // }catch (Exception ex)
                // {
                //     Console.WriteLine("Hello465576");
                //     return Result<Unit>.Failure(ex.Message);
                // }

                //  fetch the db record
                var plant = await _context.Plant.Where(x => x.plant_id == request.PlantId).ToListAsync();
                if(plant.Count<1){
                    return Result<Unit>.Failure("Invalid Plant");
                }

                //format the data to string
                var old_obj_string = new TrackerUtils().CreatePlantActivityObj(plant[0]);

                if(plant[0].status==Domain.PlantStatusOptions.ACTIVE){
                    // deactivate the plant
                    plant[0].status=Domain.PlantStatusOptions.INACTIVE;
                }else{
                    // activate the plant
                    plant[0].status=Domain.PlantStatusOptions.ACTIVE;
                }
                
                plant[0].last_updated_at = DateTime.Now;
                
                //format the data to string
                var new_obj_string = new TrackerUtils().CreatePlantActivityObj(plant[0]);
                _context.TrackingPlantActivity.Add(
                    new TrackingPlantActivity{
                        old_obj = old_obj_string,
                        new_obj = new_obj_string,
                        plant_id = plant[0].plant_id,
                        user_id = logged_user.user_id
                    }
                );
                
                // create a record in TrackerActivity
                var activity = _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = new_obj_string,
                        message = "Disabled Plant",
                        severity_type = SeverityType.CRITICAL,
                        user_id = logged_user.user_id
                    }
                );

                // Console.WriteLine("Hello");
                // save the changes to the db
                var result = await _context.SaveChangesAsync() > 0;
                // Console.WriteLine(result);

                if(!result) return Result<Unit>.Failure("Failed to deactivate the Plant");
                
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}