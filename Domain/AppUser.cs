using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain
{
    public class AppUser :  IdentityUser
    {
        public AppUser()
        {
            Photos = new HashSet<Photo>();
            Followings = new HashSet<UserFollowing>();
            Followers = new HashSet<UserFollowing>();
            UserActivities = new HashSet<UserActivity>();
        }

        public string DisplayName { get; set; }

        public string Bio { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; }

        public virtual ICollection<Photo> Photos { get; private set; }

        public virtual ICollection<UserFollowing> Followings { get; private set; }
        public virtual ICollection<UserFollowing> Followers { get;  private set; }
    }
}
