using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class ExternalLogin
    {
        public class Query : IRequest<UserDTO>
        {
            public string AccessToken { get; set; }
        };

        public class Handler : IRequestHandler<Query, UserDTO>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IFacebookAccessor faceBookAcessor;
            private readonly IJwtGenerator jwtGenerator;
            public Handler(UserManager<AppUser> userManager, IFacebookAccessor faceBookAcessor,
                IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.faceBookAcessor = faceBookAcessor;
                this.userManager = userManager;
            }

            public async Task<UserDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var userInfo = await faceBookAcessor.FacebookLogin(request.AccessToken);
                if (userInfo == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validating token" });

                var user = await userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        DisplayName = userInfo.Name,
                        Id = userInfo.Id,
                        Email = userInfo.Email,
                        UserName = "fb_" + userInfo.Id,

                    };

                    var photo = new Photo
                    {
                        Id = "fb_" + userInfo.Id,
                        IsMain = true,
                        Url = userInfo.Picture.Data.Url
                    };
                    user.Photos.Add(photo);

                    var result = await userManager.CreateAsync(user);
    
                    if (!result.Succeeded)
                        throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user" });
                }

                return new UserDTO
                {
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Token = jwtGenerator.CreateToken(user),
                    Image = user.Photos.FirstOrDefault(p=>p.IsMain == true)?.Url
                };
            }
        }
    }
}