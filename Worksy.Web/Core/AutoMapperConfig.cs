using AutoMapper;
using Worksy.Web.Data.Entities;
using Worksy.Web.DTOs;

namespace Worksy.Web.Core;

public class AutoMapperConfig: Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
    }
}