using Application.Errors;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class Delete
    {

        public class Command : IRequest
        {
            public string userName { get; set; }

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

                var observer = await context.Users.FirstOrDefaultAsync(x => x.UserName == userAcessor.GetCurrentUserName());

                var target = await context.Users.FirstOrDefaultAsync(x => x.UserName == request.userName);

                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "User not found" });

                var following = await context.Followings.FirstOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);

                if (following == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "you are not following this user" });


                context.Followings.Remove(following);

                var success = await context.SaveChangesAsync() > 0;
                if (success)
                {
                    return Unit.Value;
                }
                throw new Exception("Problem saving changes");
            }

        }

    }
}
