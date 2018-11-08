using AutoMapper;
using demoBusinessReport.Dtos;
using demoBusinessReport.Entities;

namespace demoBusinessReport.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
