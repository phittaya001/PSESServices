
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace CSI.Security.Authentication
{
    public class DefaultAuthentication : IAuthentication
    {
        protected IAuthenticationRepository Repository;
        protected bool EnableAutoAuthen;

        public DefaultAuthentication(IAuthenticationRepository repository, bool enableAutoAuthen = false)
        {
            Repository = repository;
            EnableAutoAuthen = enableAutoAuthen;
        }
        public virtual bool Authenticate(string loginName, string password)
        {
            string encrypted = SecurityModelCrypto.HashEncrypt(password);
            return Repository.TryAuthenticate(loginName, encrypted);
        }

        public virtual bool AutoAuthenticate(out string loginName)
        {
            loginName = string.Empty;
            if (EnableAutoAuthen)
            {
                try
                {
                    loginName = UserPrincipal.Current?.SamAccountName ?? string.Empty;
                    return false == string.IsNullOrEmpty(loginName);
                }
                catch (DirectoryServicesCOMException) { }
                catch (NoMatchingPrincipalException) { }
                catch (MultipleMatchesException) { }
            }
            return false;
        }
    }
}
