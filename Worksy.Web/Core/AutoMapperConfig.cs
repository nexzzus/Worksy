using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty))
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0));
    }
}