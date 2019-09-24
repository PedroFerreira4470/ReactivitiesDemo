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
    public class Delete
    {
        public class Command : IRequest
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAcessor userAcessor;
            private readonly IPhotoAcessor photoAcessor;

            public Handler(DataContext context, IUserAcessor userAcessor,
                IPhotoAcessor photoAcessor)
            {
                this.context = context;
                this.userAcessor = userAcessor;
                this.photoAcessor = photoAcessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName ==
                userAcessor.GetCurrentUserName());

                var photo = user.Photos.FirstOrDefault(x=>x.Id == request.Id);

                if (photo == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found" });

                if (photo.IsMain)
                    throw new RestException(HttpStatusCode.BadRequest, new { Photo = "You can't delete your main photo" });

                var result = photoAcessor.DeletePhoto(photo.Id);

                if (result == null)
                    throw new Exception("Problem deleting photo");

                user.Photos.Remove(photo);

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
