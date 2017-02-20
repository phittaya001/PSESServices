using CSI.Security.Authentication;
using System.Linq;
using PesWeb.Service.Security;

namespace PesWeb.Service.Security.Repositories
{
    public class AuthenticationRepo : IAuthenticationRepository<UserInformation>
    {
        public bool TryAuthenticate(string loginName, string password)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                return db.tbs_User
                    .Where(a => (a.LoginName == loginName)
                            && a.IsActive
                            && a.Password == password)
                    .Select(a => a.UserCode)
                    .Count() > 0;
            }
        }

        public UserInformation GetUserInformation(string loginName)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                return db.tbs_User
                .Where(a => (a.LoginName == loginName))
                .Select(a => new UserInformation
                {
                    UserCode = a.UserCode,
                    LoginName = a.LoginName,
                })
                .FirstOrDefault();
            }
        }
    }
}
