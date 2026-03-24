using AutoMapper;
using LicenseRemake.Domain;
using LicenseRemake.DTO.AdminPanel;
using LicenseRemake.Domain.Helpers;

namespace LicenseRemake.Infrastructure.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserDto>();
        }
    }
}
