using Domain;
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

        public ICollection<Photo> Photos { get; set; }
    }
}
