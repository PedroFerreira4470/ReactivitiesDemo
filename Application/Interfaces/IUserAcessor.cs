using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserAcessor
    {
        string GetCurrentEmail();
        string GetCurrentUserName();
    }
}
