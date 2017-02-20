using PesWeb.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace PesWeb.Service.Modules
{
    public class LDAPHelper
    {
        public string DomainName { get; set; }
        public string LdapIp { get; set; }
        public int LdapPort { get; set; }

        public LDAPHelper(string domainName, string ldapIP, int ldapPort)
        {
            DomainName = domainName;
            LdapIp = ldapIP;
            LdapPort = ldapPort;
        }

        public virtual EmployeeData Authenticate(string loginName, string password)
        {
            string path = string.Format("LDAP://{0}:{1}/DC={2};DC=COM", LdapIp, LdapPort, DomainName);
            string domain = DomainName;

            EmployeeData authenRs = new EmployeeData();
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(path))
            {
                authenRs.Result = false;
                authenRs.Message = "Invalid username or password.";
                return authenRs;
            }
            try
            {

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, loginName, password))
                using (UserPrincipal up = UserPrincipal.FindByIdentity(pc, loginName))
                {
                    authenRs.Result = true;
                    authenRs.EmployeeID = up.EmployeeId;
                    authenRs.EmployeeName = up.DisplayName;
                    return authenRs;
                }
            }
            catch (DirectoryServicesCOMException) { }
            catch (MultipleMatchesException) { }

            authenRs.Result = false;
            authenRs.Message = "Invalid username or password.";
            return authenRs;
        }
    }
}
