using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
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
    public class Login
    {

          public class Query : IRequest<User> {
            public string Email { get; set; }
            public string Password { get; set; }

        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> UserManager;
            private readonly SignInManager<AppUser> SignInManager;
            private readonly IJwtGenerator JwtGenerator;

            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                UserManager = userManager;
                SignInManager = signInManager;
                JwtGenerator = jwtGenerator;
            }

            

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await UserManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                var result = await SignInManager.CheckPasswordSignInAsync(user, request.Password,false);

                if (result.Succeeded)
                {
                    //generate Token
                    return new User {
                        DisplayName = user.DisplayName,
                        Token= JwtGenerator.CreateToken(user),
                        UserName =user.UserName,
                        Image=null,
                    };
                }
                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
