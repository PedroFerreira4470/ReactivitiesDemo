using Application.Followers;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/profiles")]
    public class FollowersController : BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<Unit> Follow(string userName) {
            return await Mediator.Send(new Add.Command { userName = userName }); ;
        }

        [HttpDelete("{username}/follow")]
        public async Task<Unit> Unfollow(string userName)
        {
            return await Mediator.Send(new Delete.Command { userName = userName }); ;
        }

        [HttpGet("{username}/follow")]
        public async Task<ActionResult<List<ProfileDTO>>> GetFollowings(string userName, string predicate)
        {
            return await Mediator.Send( new List.Query{UserName = userName, Predicate = predicate}); 
        }

    }
}
