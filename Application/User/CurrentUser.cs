using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User
{
    public class CurrentUser
    {
        public class Query : IRequest<User>
        {
            
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwt;
            private readonly IUserAcessor userAcessor;

            public Handler(UserManager<AppUser> userManager, IJwtGenerator jwt, IUserAcessor userAcessor)
            {
                this.userManager = userManager;
                this.jwt = jwt;
                this.userAcessor = userAcessor;
            }
            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {

                var user = await userManager.FindByNameAsync(userAcessor.GetCurrentUserName());

                if (user == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not Found" });

                return new User
                {
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Token = jwt.CreateToken(user),
                    Image = null,
                };
            }
        }

    }
}
