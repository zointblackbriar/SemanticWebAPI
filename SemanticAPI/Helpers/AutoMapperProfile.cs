using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemanticAPI.Dtos;
using SemanticAPI.Entities;

using AutoMapper;


//Mapping Data Transfer Object and User Information
//AutoMapper Profile is added
//The automapper profile contains the mapping configuration used by the application, it enables mapping
//of user entities to dtos and dtos to entities
namespace SemanticAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
