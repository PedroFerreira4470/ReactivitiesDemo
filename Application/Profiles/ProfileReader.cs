using Application.Errors;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly DataContext context;
        private readonly IUserAcessor userAcessor;
        private readonly IMapper mapper;

        public ProfileReader(DataContext context, IUserAcessor userAcessor, IMapper mapper)
        {
            this.context = context;
            this.userAcessor = userAcessor;
            this.mapper = mapper;
        }

        public async Task<ProfileDTO> ReadProfile(string userName)
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.UserName == userName);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest,new { User ="User not found"});

            var currentUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == userAcessor.GetCurrentUserName());

            var profile = mapper.Map<AppUser, ProfileDTO>(user);

            return profile;

        }
    }
}
