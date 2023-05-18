using Application.Core;
using Domain;
using MediatR;
using FluentValidation;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Application.DTOs;

namespace Application.Plants
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>{
            public PlantDto plant { get; set; } 
            public Guid product_id { get; set; }

            public User logged_user {get; set;}
        }
        
        // public class CommandValidator : AbstractValidator<Command>
        // {
        //     public CommandValidator()
        //     {
        //         RuleFor(x => x.plant).SetValidator(new PlantValidator());
        //     }
        // }


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

                // check whether product exists or not
                var product = await _context.ProductManagement.FindAsync(request.product_id);

                if(product==null) return null;

                var admin = new User();
                if(request.plant.operated_id != null){
                    // Console.WriteLine("if");
                    // Guid operated_id;
                    // try{
                    //     operated_id =(Guid) request.plant.operated_id;
                    // }catch {
                    //     operated_id = Guid.NewGuid();
                    // }
                    // Console.WriteLine(operated_id);
                    // check whether given plant admin exists or not

                    // TODO : Check whether the given plant admin exists or not 

                    var admins =  _context.User.Where(x => x.user_id == request.plant.operated_id).ToList();
                    // Console.WriteLine("Heloooooooooooooooo");
                    if(admins.Count < 1) return Result<Unit>.Failure("Invalid operated_id");

                    admin = admins[0];
                    

                    // check whether the given plant_code is unique or not
                    var plant = await _context.Plant.Where(x => (x.plant_code == request.plant.plant_code) | (x.operated_id== request.plant.operated_id)).ToListAsync();
                    if(plant.Count > 0) return Result<Unit>.Failure("plant_code/ Admin is used");
                }else{
                    // Console.WriteLine("else");
                    // check whether the given plant_code is unique or not
                    var plant = await _context.Plant.Where(x => x.plant_code == request.plant.plant_code).ToListAsync();
                    if(plant.Count > 0) return Result<Unit>.Failure("plant_code/ Admin is used");
                }

                //  convert the founded_on value type from string to datetime 
                request.plant.founded_on =(DateTime) request.plant.founded_on;

                var new_plant_record = _context.Plant.Add(new Plant{
                    plant_name = request.plant.plant_name,
                    plant_code = request.plant.plant_code,
                    plant_description = request.plant.plant_description,
                    plant_location_address = request.plant.plant_location_address,
                    plant_location_city = request.plant.plant_location_city,
                    plant_location_state = request.plant.plant_location_state,
                    plant_location_country = request.plant.plant_location_country,
                    plant_location_pincode = request.plant.plant_location_pincode,
                    plant_location_geo = request.plant.plant_location_geo,
                    plant_qr_limit = request.plant.plant_qr_limit,
                    created_by = logged_user.user_id,
                    operated_id = request.plant.operated_id,
                    founded_on = request.plant.founded_on
                });

                var result = await _context.SaveChangesAsync()>0;
                if(!result) return Result<Unit>.Failure("Failed to create Activity");

                //  fetch the plant_id of the plant we had created above
                // var new_plant = await _context.Plant.Where(x => (x.plant_code == request.plant.plant_code)).ToListAsync();
                // var new_plant_id = new_plant[0].plant_id;
                var new_plant = new_plant_record.Entity;
                var new_plant_id = new_plant.plant_id;

                if(request.plant.operated_id != null){
                    // add the plant_id to the Operated user
                    admin.plant_id = new_plant_id;
                }

                //  create the record in ProductPlantmapping
                _context.ProductPlantMapping.Add( new ProductPlantMapping{
                    product_id = request.product_id,
                    plant_id = new_plant_id,
                    // max_qr_limit = request.plant.plant_qr_limit,
                    status = PlantStatusOptions.ACTIVE,
                    created_by = logged_user.user_id,
                    created_at = DateTime.Now,
                    last_updated_at = DateTime.Now
                });

                
                var result2 = await _context.SaveChangesAsync()>0;
                if(!result2){
                    // delete the new plant that was created above
                    _context.Remove(new_plant);

                    await _context.SaveChangesAsync();

                    return Result<Unit>.Failure("Failed to create Plant");
                } 

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}