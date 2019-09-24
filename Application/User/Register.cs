using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Validators;

namespace Application.User
{
    public class Register
    {

        public class Command : IRequest<UserDTO>
        {
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

        }

        public class Commandvalidation : AbstractValidator<Command>
        {
            public Commandvalidation()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).PasswordValidator();

            }
        }

        public class Handler : IRequestHandler<Command,UserDTO>
        {
            private readonly DataContext context;
            private readonly IJwtGenerator jwt;
            private readonly UserManager<AppUser> userManager;
            public Handler(DataContext context,UserManager<AppUser> userManager,IJwtGenerator jwt)
            {
                this.context = context;
                this.jwt = jwt;
                this.userManager = userManager;

            }

            public async Task<UserDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await context.Users.Where(p => p.Email == request.Email).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest,"Email Already Exist");
                if (await context.Users.Where(p => p.UserName == request.UserName).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, "UserName Already Exist");

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    UserName= request.UserName,
                    Email = request.Email,
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return new UserDTO {
                        DisplayName = request.DisplayName,
                        UserName = request.UserName,
                        Token = jwt.CreateToken(user),
                        Image = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                    };
                }
                throw new Exception("Problem creating user");
            }
        }
    }
}
