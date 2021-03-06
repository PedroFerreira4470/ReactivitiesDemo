﻿using Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
   
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(Login.Query query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserDTO>> FacebookLogin(ExternalLogin.Query query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(Register.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<UserDTO>> CurrentUser()
        {
            return await Mediator.Send(new CurrentUser.Query());
        }
    }
}
