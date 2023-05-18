using Application.Core;
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

                if(plant[0].status==Domain.PlantStatusOptions.ACTIVE){
                    // deactivate the plant
                    plant[0].status=Domain.PlantStatusOptions.INACTIVE;
                }else{
                    // activate the plant
                    plant[0].status=Domain.PlantStatusOptions.ACTIVE;
                }
                
                plant[0].last_updated_at = DateTime.Now;
                
                
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