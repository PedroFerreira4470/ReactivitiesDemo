using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor httpContextAcessor;
        private readonly DataContext dataContext;

        public IsHostRequirementHandler(IHttpContextAccessor httpContextAcessor,
            DataContext dataContext) {
            this.httpContextAcessor = httpContextAcessor;
            this.dataContext = dataContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        IsHostRequirement requirement)
        {

            var currentUserName = httpContextAcessor.HttpContext.User?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var activityId = Guid.Parse(httpContextAcessor.HttpContext.Request.RouteValues
                .SingleOrDefault(x=>x.Key == "id")
                .Value.ToString());
            //var activityId = Guid.Parse(authContenxt.RouteData.Values["id"].ToString());

            var activity = dataContext.Activities.FindAsync(activityId).Result;

            var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

            if (host?.AppUser?.UserName == currentUserName)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
