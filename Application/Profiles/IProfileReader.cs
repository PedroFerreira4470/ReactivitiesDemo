
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public interface IProfileReader
    {
        Task<ProfileDTO> ReadProfile(string userName);
    }
}
