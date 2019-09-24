using Application.Errors;
using Application.Interfaces;
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

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest
        {
            public string Id { get; set; }
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
                var user =  await context.Users.FirstOrDefaultAsync(x => x.UserName == userAcessor.GetCurrentUserName());

                var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);

                if (photo == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if (currentMain != null)
                    currentMain.IsMain = false;

                photo.IsMain = true;

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
