﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles
{
    public class UserActivityDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }
}
