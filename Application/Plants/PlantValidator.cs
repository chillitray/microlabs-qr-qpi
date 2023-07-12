using Domain;
using FluentValidation;

namespace Application.Plants
{
    public class PlantValidator : AbstractValidator<Plant>
    {
        public PlantValidator()
        {
            RuleFor(x => x.plant_name).NotEmpty();
            RuleFor(x => x.plant_code).NotEmpty();
            RuleFor(x => x.plant_description).NotEmpty();
            RuleFor(x=>x.plant_location_address).NotEmpty();
            // RuleFor(x=>x.plant_location_city).NotEmpty();
            // RuleFor(x=>x.plant_location_state).NotEmpty();
            // RuleFor(x=>x.plant_location_country).NotEmpty();
            // RuleFor(x=>x.plant_location_pincode).NotEmpty();
            // RuleFor(x=>x.plant_location_geo).NotEmpty();
            RuleFor(x=>x.plant_qr_limit).NotEmpty();
            RuleFor(x=>x.created_by).NotEmpty();
            // RuleFor(x=>x.status).NotEmpty().IsInEnum();
            RuleFor(x=>x.founded_on).NotEmpty();
            RuleFor(x => x.operated_id).NotEmpty();
            // RuleFor(x=>x.created_at).NotEmpty();
            // RuleFor(x=>x.last_updated_at).NotEmpty();
        }
    }
}