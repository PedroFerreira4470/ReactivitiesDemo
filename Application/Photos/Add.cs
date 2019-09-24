using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Photo>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Photo>
        {
            private readonly DataContext context;
            private readonly IUserAcessor userAcessor;
            private readonly IPhotoAcessor photoAcessor;

            public Handler(DataContext context, IUserAcessor userAcessor, IPhotoAcessor photoAcessor)
            {
                this.context = context;
                this.userAcessor = userAcessor;
                this.photoAcessor = photoAcessor;
            }

            public async Task<Photo> Handle(Command request, CancellationToken cancellationToken)
            {
                var photoUploadResult = photoAcessor.AddPhoto(request.File);

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName ==
                    userAcessor.GetCurrentUserName());

                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId,
                };

                if (!user.Photos.Any(x=>x.IsMain))
                {
                    photo.IsMain = true;
                }

                user.Photos.Add(photo);

                // Handle Logic goes here
                var success = await context.SaveChangesAsync() > 0;
                if (success)
                {
                    return photo;
                }
                throw new Exception("Problem saving changes");
            }
        }
    }
}
