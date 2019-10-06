using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using AutoMapper;
using System.Linq;
using System;
using Application.Interfaces;

namespace Application.Activities
{
    public class List
    {
        public class ActivitiesEnvelope
        {
            public List<ActivityDTO> Activities { get; set; }
            public int ActivityCount { get; set; }
        }
        public class Query : IRequest<ActivitiesEnvelope>
        {
            public int Limit { get; set; }
            public int OffSet { get; set; }
            public bool IsGoing { get; }
            public bool IsHost { get; }
            public DateTime? StartDate { get; }

            public Query(int? limit, int? offSet, bool isGoing, bool isHost, DateTime? startDate)
            {
                this.Limit = limit ?? 3;
                this.OffSet = offSet ?? 0;
                this.IsGoing = isGoing;
                this.IsHost = isHost;
                this.StartDate = startDate ?? DateTime.UtcNow;
            }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            private readonly IUserAcessor userAcessor;

            public Handler(DataContext context, IMapper mapper, IUserAcessor userAcessor)
            {
                this.context = context;
                this.mapper = mapper;
                this.userAcessor = userAcessor;
            }
            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {

                var queryable = context.Activities
                    .Where(x => x.Date >= request.StartDate)
                    .OrderBy(x=>x.Date)
                    .AsQueryable();

                if (request.IsGoing)
                {
                    queryable = queryable.Where(x => x.UserActivities.Any(u => u.AppUser.UserName == userAcessor.GetCurrentUserName()));
                }

                if (request.IsHost)
                {
                    queryable = queryable.Where(x => x.UserActivities.Any(u => u.AppUser.UserName == userAcessor.GetCurrentUserName() && u.IsHost));
                }

                var activities = await queryable
                    .Skip(request.OffSet)
                    .Take(request.Limit)
                    .ToListAsync();

                var activitiesDTO = mapper.Map<List<Activity>, List<ActivityDTO>>(activities);

                return new ActivitiesEnvelope {
                       Activities = activitiesDTO,
                       ActivityCount = queryable.Count(),
                };
            }
        }
    }
}