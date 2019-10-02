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

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class Commandvalidation : AbstractValidator<Command>
        {
            public Commandvalidation()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
            }
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
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userAcessor.GetCurrentUserName());

                if (user == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not Found" });

                user.DisplayName = request.DisplayName;
                user.Bio = request.Bio;
                
                // Handle Logic goes here
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
