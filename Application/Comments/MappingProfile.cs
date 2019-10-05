using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Comments
{
   public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Comment, CommentDTO>()
                .ForMember(x => x.CreateAt, o => o.MapFrom(p => p.CreateAt.ToLocalTime()))
                .ForMember(x => x.UserName, o => o.MapFrom(p => p.Author.UserName))
                .ForMember(x => x.DisplayName, o => o.MapFrom(p => p.Author.DisplayName))
                .ForMember(x => x.Image, o => o.MapFrom(p => p.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}
