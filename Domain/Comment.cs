using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public virtual AppUser Author { get; set; }
        public virtual Activity Activity { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
