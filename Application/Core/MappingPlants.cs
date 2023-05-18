using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingPlants : Profile
    {
        public MappingPlants()
        {
            CreateMap<Plant, Plant>();
        }
    }
}