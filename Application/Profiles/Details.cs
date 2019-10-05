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
            private readonly IProfileReader profileReader;

            public Handler(IProfileReader profileReader)
            {
                this.profileReader = profileReader;
            }
            public async Task<ProfileDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                return await profileReader.ReadProfile(request.UserName);
            }
        }
    }
}
