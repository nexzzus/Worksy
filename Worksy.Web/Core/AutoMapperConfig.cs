using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;
using Worksy.Web.ViewModels;

namespace Worksy.Web.Core
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // User - DTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // nunca exponer la contraseña

            // DTO - User
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) =>
                        srcMember != null && !(srcMember is string s && string.IsNullOrWhiteSpace(s))
                    ));

            CreateMap<User, UpdateProfileDTO>().ReverseMap();
            CreateMap<User, RegisterViewModel>().ReverseMap();

            // De entidad a DTO
            CreateMap<Category, CategoryDTO>();

            // De DTO a entidad
            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Services, opt => opt.Ignore()); // Evita sobrescribir la relación

            // De entidad a DTO
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.User,
                    opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.Categories
                        .Select(c => c.CategoryId)
                        .ToList()));
            
            CreateMap<ServiceDTO, Service>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Permissions
            CreateMap<Permission, PermissionDTO>();

            // Roles
            CreateMap<WorksyRole, WorksyRoleDTO>().ReverseMap();
        }
    }
}