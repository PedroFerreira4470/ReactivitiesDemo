using Application.Errors;
using Application.Interfaces;
using AutoMapper;
using Domain;
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

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<CommentDTO>
        {
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
            public string UserName { get; set; }

        }

        public class Commandvalidation : AbstractValidator<Command>
        {
            public Commandvalidation()
            {
                RuleFor(x => x.Body).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command,CommentDTO>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context,IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<CommentDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync(request.ActivityId);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity="Not Found!"});

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

                var comment = new Comment {
                    Activity = activity,
                    Author = user,
                    Body = request.Body,
                    CreateAt = DateTime.UtcNow,
                };

                context.Comments.Add(comment);

                var success = await context.SaveChangesAsync() > 0;
                if (success)
                {
                    var t = mapper.Map<CommentDTO>(comment);
                    return t;
                }
                throw new Exception("Problem saving changes");
            }
        }
    }
}
