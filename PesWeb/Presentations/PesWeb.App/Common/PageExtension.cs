using CSI.CastleWindsorHelper;
using CSI.Security.Authentication;
using CSI.Security.Authorization;
using PesWeb.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

namespace PesWeb.App.Common
{
    public static class PageExtension
    {
        public static UserInformation GetInformation(this IPrincipal user)
        {
            return GetUserInformation(null, user.Identity.Name);
        }
        public static UserInformation GetUserInformation(this Page page, string loginName)
        {
            var info = HttpContext.Current.Session[ConstSessionID.UserInformationObject] as UserInformation;
            if (null == info)
            {
                IAuthenticationRepository<UserInformation> svc;
                if (ServiceContainer.TryGetService<IAuthenticationRepository<UserInformation>>(out svc))
                    info = svc.GetUserInformation(loginName);
                else
                    info = new UserInformation();

                HttpContext.Current.Session[ConstSessionID.UserInformationObject] = info;
            }
            return info;
        }
    }
}