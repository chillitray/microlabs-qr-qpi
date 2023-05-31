using Application.Core;
using Application.Trackers;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Plants
{
    public class EditPlant
    {
        public class Command : IRequest<Result<Unit>>{
            public Plant plant { get; set; }
            public User logged_user {get; set;}
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.plant).SetValidator(new PlantValidator());
            }
        }

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
                
                activity.last_updated_at = DateTime.Now;
                _mapper.Map(request.plant, activity);

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

                var result = await _context.SaveChangesAsync()>0;

                if(!result) return Result<Unit>.Failure("Failed to update the Plant");
                
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}