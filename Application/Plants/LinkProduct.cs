using Application.Core;
using Application.Trackers;
using Domain;
using MediatR;
using Persistence;

namespace Application.Plants
{
    public class LinkProduct
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
                
                //verify whether the plant is in active state or not
                var plant =await _context.Plant.FindAsync(request.PlantId);
                if(plant==null){
                    return Result<Unit>.Failure("Invalid Plant");
                }
                if(plant.status != Domain.PlantStatusOptions.ACTIVE){
                    return Result<Unit>.Failure("Invalid Plant");
                }

                //verify whether the product is in active state or not
                var product =await _context.ProductManagement.FindAsync(request.ProductId);
                if(product==null){
                    return Result<Unit>.Failure("Invalid product");
                }
                if(product.status != Domain.PlantStatusOptions.ACTIVE){
                    return Result<Unit>.Failure("Invalid product");
                }
                


                // verify whether product-plant is already mapped
                var activity = _context.ProductPlantMapping.Where(x => x.plant_id==request.PlantId & x.product_id == request.ProductId).ToList();
                
                // Console.WriteLine("Hello");
                if(activity.Count>0) return Result<Unit>.Failure("Already Mapped");
                
                //  create the record in ProductPlantmapping
                var new_mapping_record = _context.ProductPlantMapping.Add( new ProductPlantMapping{
                    product_id = request.ProductId,
                    plant_id = request.PlantId,
                    //  TODO : Change the below static value to dynamic after Auth integration
                    created_by = new Guid("930e0d95-7a3f-4bb5-abc3-08db5165c970")
                });
                
                //format the data to string
                var new_obj_string2 = new TrackerUtils().CreateProductPlantmappingObj(new_mapping_record.Entity);
                _context.TrackingProductPlantMapActivity.Add(
                    new TrackingProductPlantMapActivity{
                        new_obj = new_obj_string2,
                        product_plant_mapping_id = new_mapping_record.Entity.product_plant_mapping_id,
                        user_id = logged_user.user_id
                    }
                );
                
                // Console.WriteLine("Hello");

                // save the changes to the DB
                var result = await _context.SaveChangesAsync() > 0;
                // Console.WriteLine(result);
                if(!result) return Result<Unit>.Failure("Failed to Map");
                
                return Result<Unit>.Success(Unit.Value);
            }

        }
    }
}