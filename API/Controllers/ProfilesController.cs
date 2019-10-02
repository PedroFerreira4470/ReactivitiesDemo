using Application.Photos;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ProfilesController : BaseController
    {
        [HttpGet("{userName}")]
        public async Task<ActionResult<ProfileDTO>> Get(string userName) {
            return await Mediator.Send(new Details.Query { UserName = userName });
        }

        [HttpPut]
        public async Task<ActionResult<Unit>> Edit(Edit.Command command) {
            return await Mediator.Send(command);
        }

    }
}
