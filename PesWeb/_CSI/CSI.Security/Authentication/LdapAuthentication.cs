using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace CSI.Security.Authentication
{
    public class LdapAuthentication : IAuthentication
    {
        public string DomainName { get; set; }
        public string LdapIp { get; set; }
        public int LdapPort { get; set; }

        public LdapAuthentication(string domainName, string ldapIP, int ldapPort)
        {
            DomainName = domainName;
            LdapIp = ldapIP;
            LdapPort = ldapPort;
        }
        public virtual bool Authenticate(string loginName, string password)
        {
            string path = string.Format("LDAP://{0}:{1}/DC={2};DC=COM", LdapIp, LdapPort, DomainName);
            string domain = DomainName;

            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(path))
                return false;

            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, loginName, password))
                using (UserPrincipal up = UserPrincipal.FindByIdentity(pc, loginName))
                {
                    return null != up;
                }
            }
            catch (DirectoryServicesCOMException) { }
            catch (MultipleMatchesException) { }

            return false;
        }

        public virtual bool AutoAuthenticate(out string loginName)
        {
            loginName = string.Empty;
            try
            {
                loginName = UserPrincipal.Current?.SamAccountName ?? string.Empty;
                return false == string.IsNullOrEmpty(loginName);
            }
            catch (DirectoryServicesCOMException) { }
            catch (NoMatchingPrincipalException) { }
            catch (MultipleMatchesException) { }

            return false;
        }
    }
}
