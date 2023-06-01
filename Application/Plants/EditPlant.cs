using Application.Core;
using Application.DTOs;
using Application.Trackers;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Plants
{
    public class EditPlant
    {
        public class Command : IRequest<Result<Unit>>{
            public EditPlantDto plant { get; set; }
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
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this._mapper = mapper;
                this._context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var logged_user = request.logged_user;
                var activity = await _context.Plant.FindAsync(request.plant.plant_id);

                if(activity==null) return null;
                //format the data to string
                var old_obj_string = new TrackerUtils().CreatePlantActivityObj(activity);

                if(request.plant.operated_id!=null){
                    //verify the user
                    var plant = await _context.Plant.Where(x => x.operated_id== request.plant.operated_id).ToListAsync();
                    if(plant.Count>0) return Result<Unit>.Failure("operated_id is already taken");
                    var user_check = _context.User.Where(x => x.user_id==request.plant.operated_id).ToList();
                    if(user_check.Count<0) return Result<Unit>.Failure("operated_id is invalid");
                }
                
                activity.last_updated_at = DateTime.Now;
                // _mapper.Map(request.plant, activity);
                activity.plant_name = request.plant.plant_name ?? activity.plant_name;
                activity.plant_code = request.plant.plant_code ?? activity.plant_code;
                activity.plant_description = request.plant.plant_description ?? activity.plant_description;
                activity.plant_location_address = request.plant.plant_location_address ?? activity.plant_location_address;
                activity.plant_location_city = request.plant.plant_location_city ?? activity.plant_location_city;
                activity.plant_location_country = request.plant.plant_location_country ?? activity.plant_location_country;
                activity.plant_location_geo = request.plant.plant_location_geo ?? activity.plant_location_geo;
                activity.plant_location_pincode = request.plant.plant_location_pincode ?? activity.plant_location_pincode;
                activity.plant_location_state = request.plant.plant_location_state ?? activity.plant_location_state;
                activity.operated_id = request.plant.operated_id ?? activity.operated_id;
                activity.founded_on = request.plant.founded_on ?? activity.founded_on;
                if( activity.plant_qr_limit!=0){
                    activity.plant_qr_limit = request.plant.plant_qr_limit;
                }
                

                //format the data to string
                var new_obj_string = new TrackerUtils().CreatePlantActivityObj(activity);
                _context.TrackingPlantActivity.Add(
                    new TrackingPlantActivity{
                        old_obj = old_obj_string,
                        new_obj = new_obj_string,
                        plant_id =activity.plant_id,
                        user_id = logged_user.user_id
                    }
                );

                // create a record in TrackerActivity
                _context.TrackingActivity.Add(
                    new TrackingActivity{
                        custom_obj = new_obj_string,
                        message = "Edited plant details",
                        severity_type = SeverityType.CRITICAL,
                        user_id = logged_user.user_id
                    }
                );

                var result = await _context.SaveChangesAsync()>0;

                if(!result) return Result<Unit>.Failure("Failed to update the Plant");
                
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}