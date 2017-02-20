using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Security.Authentication
{
    public interface IAuthenticationRepository
    {
        bool TryAuthenticate(string loginName, string password);
    }
    public interface IAuthenticationRepository<T> : IAuthenticationRepository
    {
        T GetUserInformation(string loginName);
    }
}
