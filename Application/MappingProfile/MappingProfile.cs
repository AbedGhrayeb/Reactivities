using System.Linq;
using Application.Activities.Dtos;
using AutoMapper;
using Domain;


namespace Application.MappingProfile
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest=> dest.Image,opt => opt.MapFrom(src=>src.User.Photos.FirstOrDefault(x=>x.IsMain).Url));
        }
        
    }
}
