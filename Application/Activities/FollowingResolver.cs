﻿using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Activities
{
    public class FollowingResolver : IValueResolver<UserActivity, AttendeeDTO, bool>
    {
        private readonly DataContext context;
        private readonly IUserAcessor userAcessor;

        public FollowingResolver(DataContext context ,IUserAcessor userAcessor)
        {
            this.context = context;
            this.userAcessor = userAcessor;
        }

        public bool Resolve(UserActivity source, AttendeeDTO destination, bool destMember, ResolutionContext context)
        {
            var currentUser = this.context.Users.FirstOrDefaultAsync(x => x.UserName == userAcessor.GetCurrentUserName()).Result;

            if (currentUser.Followings.Any(x=>x.TargetId == source.AppUserId))
            {
                return true;
            }
            return false;
        }
    }
}
