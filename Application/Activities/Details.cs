using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
          public class Query : IRequest<Activity>
          {
              public Guid Id { get; set; }
          }

        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext context;
            public Handler(DataContext context){
                this.context = context;
            }
            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync(request.Id);
                return activity;
            }
        }
    }
}