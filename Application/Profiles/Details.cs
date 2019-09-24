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

namespace Application.Profiles
{
    public class Details
    {
        public class Query : IRequest<ProfileDTO>
        {
            public string UserName { get; set; }
        }
        
        public class Handler : IRequestHandler<Query, ProfileDTO>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context , IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }
            public async Task<ProfileDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

                return mapper.Map<AppUser, ProfileDTO>(user);
            }
        }
    }
}
