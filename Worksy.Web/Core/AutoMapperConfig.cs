using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;

namespace Worksy.Web.Core
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // De entidad a DTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // nunca exponer la contraseña

            // De DTO a entidad
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                // ✅ Ignorar propiedades nulls o vacías para no sobrescribir datos existentes
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null && !(srcMember is string s && string.IsNullOrWhiteSpace(s))
                ));
            
            CreateMap<User, UpdateProfileDTO>();

            // Mapeo inverso si necesitas actualizar desde DTO
            CreateMap<UpdateProfileDTO, User>();
        }
    }
}