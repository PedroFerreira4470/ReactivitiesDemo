using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles
{
    public class ProfileDTO
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        [JsonProperty("following")]
        public bool IsFollowed { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
