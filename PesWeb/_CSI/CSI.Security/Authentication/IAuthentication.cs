using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace CSI.Security.Authentication
{
    public interface IAuthentication
    {
        bool Authenticate(string loginName, string password);
        bool AutoAuthenticate(out string loginName);
    }

    public interface IAuthentication<T> : IAuthentication
    {
        bool Authenticate(string loginName, string password, ref T customData);
        string AutoAuthenticate(out string loginName, ref T customData);
    }
}
