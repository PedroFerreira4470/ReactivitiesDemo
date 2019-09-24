using AutoMapper;
using Domain;
using System.Linq;

namespace Application.Profiles
{
   public class MappingProfile : AutoMapper.Profile
    {

        public MappingProfile()
        {
            CreateMap<AppUser, ProfileDTO>()
                .ForMember(destination => destination.Image, option => option.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain == true).Url));

        }
    }
}
