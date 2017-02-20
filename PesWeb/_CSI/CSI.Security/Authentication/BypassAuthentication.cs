using CSI.Security.Authorization;

namespace CSI.Security.Authentication
{
    public class BypassAuthentication : IAuthentication
    {
        public bool Authenticate(string loginName, string password)
        {
            return true;
        }

        public bool AutoAuthenticate(out string loginName)
        {
            loginName = string.Empty;
            return false;
        }
    }
}
