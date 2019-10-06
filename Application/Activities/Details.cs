using Application.Errors;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ActivityDTO>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDTO>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }
            public async Task<ActivityDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities
                    .FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not Found" });

                var activityDTO = mapper.Map<Activity, ActivityDTO>(activity);

                return activityDTO;
            }
        }
    }
}