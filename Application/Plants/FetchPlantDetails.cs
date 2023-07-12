using Application.Core;
using Application.DTOs;
using Domain;
using MediatR;
using Persistence;

namespace Application.Plants
{
    public class FetchPlantDetails
    {
        public class Query : IRequest<Result<PlantDetailsDto>> {
            public Guid PlantId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PlantDetailsDto>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            { 
                this._context = context;
                
            }

            public async Task<Result<PlantDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                //  fetch the plant details
                var plant_details =await _context.Plant.FindAsync(request.PlantId);
                // fetch the products linked to this plant
                var product_ids = _context.ProductPlantMapping.Where(x => x.plant_id == request.PlantId & x.status == PlantStatusOptions.ACTIVE).Select(x => x.product_id).ToList();

                // fetch the product details
                List<ProductDetails> products = _context.ProductManagement.Where(x => product_ids.Contains(x.product_id)).Select(x=> new ProductDetails(x.product_id,x.product_name)).ToList();

                var created_user = plant_details.created_by;
                var operated_id = plant_details.operated_id;

                var users = _context.User.Where(x => x.user_id == created_user | x.user_id == operated_id).Select(x => new{x.user_id,x.full_name,x.Email}).ToList();
                var created_by_name = "";
                var operated_by_name = "";

                // Console.WriteLine("hello");
                try
                {
                    created_by_name =  users.Find(x => x.user_id==created_user).full_name;
                }
                catch (System.Exception)
                {
                    
                    created_by_name = "";
                }                
                // Console.WriteLine(created_by_name);
                try
                {
                    operated_by_name = users.Find(x =>x.user_id == operated_id).full_name;
                }
                catch (System.Exception)
                {
                    
                    operated_by_name="";
                }                
                // Console.WriteLine(operated_by_name);
                var status = plant_details.status == Domain.PlantStatusOptions.ACTIVE ? "ACTIVE" : "INACTIVE";
                var output = new PlantDetailsDto(
                    plant_details.plant_id,
                    plant_details.plant_name,
                    plant_details.plant_code,
                    plant_details.plant_description,
                    plant_details.plant_location_address,
                    // plant_details.plant_location_city,
                    // plant_details.plant_location_state,
                    // plant_details.plant_location_country,
                    // plant_details.plant_location_pincode,
                    // plant_details.plant_location_geo,
                    plant_details.plant_qr_limit,
                    created_by_name,
                    plant_details.created_by,
                    plant_details.operated_id,
                    operated_by_name,
                    status,
                    plant_details.founded_on,
                    plant_details.created_at,
                    plant_details.last_updated_at,
                    products
                );

                return Result<PlantDetailsDto>.Success(output);


            }
        }
    }
}