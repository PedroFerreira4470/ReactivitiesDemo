using AutoMapper;
using Domain;
using System.Linq;

namespace Application.Profiles
{
   public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<AppUser, ProfileDTO>()
                .ForMember(destination => destination.Image, option => option.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain == true).Url))
                .ForMember(destination => destination.FollowersCount, option => option.MapFrom(source => source.Followers.Count()))
                .ForMember(destination => destination.FollowingCount, option => option.MapFrom(source => source.Followings.Count()))
                .ForMember(destination => destination.IsFollowed, option => option.MapFrom<FollowingResolver>());
        }
    }
}
