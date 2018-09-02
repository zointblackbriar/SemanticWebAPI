using AutoMapper;
using SemanticAPI.Dtos;
using SemanticAPI.Entities;

namespace SemanticAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

        }
    }
}
