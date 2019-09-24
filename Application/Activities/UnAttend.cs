using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Unattend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAcessor userAcessor;

            public Handler(DataContext context, IUserAcessor userAcessor)
            {
                this.context = context;
                this.userAcessor = userAcessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Activity not found" });

                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userAcessor.GetCurrentUserName());

                if (user == null) /*Not really needed*/
                    throw new RestException(HttpStatusCode.NotFound, new { user = "UserDTO not found" });


                var attendance = await context.UserActivities
                    .FirstOrDefaultAsync(a =>
                    a.ActivityId == activity.Id &&
                    a.AppUserId == user.Id);

                if (attendance == null) 
                    return Unit.Value;

                if (attendance.IsHost)
                    throw new RestException(HttpStatusCode.BadRequest, new { Attendee = "You can't remove yourself as Host" });

                context.UserActivities.Remove(attendance);

                var success = await context.SaveChangesAsync() > 0;

                if (success) { return Unit.Value; }

                throw new Exception("Problem saving changes");
            }
        }
    }
}
