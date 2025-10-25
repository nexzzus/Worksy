using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.ViewModels;

namespace Worksy.Web.Core
{
    public class AutoMapperConfig : Profile
    {
        // TODO: Completar mapeos según sea necesario
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

            CreateMap<User, RegisterViewModel>().ReverseMap();

            // Mapeo inverso si necesitas actualizar desde DTO
            CreateMap<UpdateProfileDTO, User>();

            // De entidad a DTO
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));
            // De DTO a entidad
            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Services, opt => opt.Ignore()); // Evita sobrescribir la relación

            // De entidad a DTO
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));
            // De DTO a entidad
            CreateMap<ServiceDTO, Service>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore()); // Evita sobrescribir la relación
        }

    }
}