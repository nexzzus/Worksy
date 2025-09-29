using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.Ignore()) // Nunca mandamos Password desde BD
            .ReverseMap()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // PasswordHash no lo toca AutoMapper
    }
}