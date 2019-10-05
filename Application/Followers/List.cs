using Application.Profiles;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<List<ProfileDTO>>
        {
            public string UserName { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ProfileDTO>>
        {
            private readonly DataContext context;
            private readonly IProfileReader profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                this.context = context;
                this.profileReader = profileReader;
            }
            public async Task<List<ProfileDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Followings.AsQueryable();
                var userFollowings = new List<UserFollowing>();
                var profiles = new List<ProfileDTO>();

                switch (request.Predicate)
                {
                    case "followers":
                    {
                        userFollowings = await queryable.Where(x => x.Target.UserName == request.UserName).ToListAsync();
                        foreach (var follower in userFollowings)
                        {
                            profiles.Add(await profileReader.ReadProfile
                                    (follower.Observer.UserName));
                        }
                            break;
                    }
                    case "following":
                        {
                            userFollowings = await queryable.Where(x => x.Observer.UserName == request.UserName).ToListAsync();
                            foreach (var follower in userFollowings)
                            {
                                profiles.Add(await profileReader.ReadProfile
                                        (follower.Target.UserName));
                            }
                            break;
                        }
                }
                return profiles;
            }
        }
    }
}
