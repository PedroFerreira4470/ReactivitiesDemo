using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDTO>();
            CreateMap<UserActivity, AttendeeDTO>()
                .ForMember(destination=> destination.UserName,option=> option.MapFrom(source=> source.AppUser.UserName))
                .ForMember(destination => destination.DisplayName, option => option.MapFrom(source => source.AppUser.DisplayName));

        }
    }
}
