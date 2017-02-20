using CSI.CastleWindsorHelper;
using CSI.Security.Authentication;
using CSI.Web.UI.Common;
using PesWeb.App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PesWeb.App.Forms.Security
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (false == Page.User.Identity.IsAuthenticated)
            {
                var auth = ServiceContainer.GetService<IAuthentication>();
                string loginName;
                if (auth.AutoAuthenticate(out loginName))
                {
                    this.GetUserInformation(loginName);
                    FormsAuthentication.SetAuthCookie(loginName, false);
                    this.RegisterSessionOnwer(loginName);
                    Response.Redirect(FormsAuthentication.DefaultUrl);
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string loginName = txtLoginName.Text;
            var auth = ServiceContainer.GetService<IAuthentication>();
            if (auth.Authenticate(loginName, txtPassword.Text))
            {
                this.GetUserInformation(loginName);
                FormsAuthentication.SetAuthCookie(loginName, false);
                this.RegisterSessionOnwer(loginName);
                Response.Redirect(FormsAuthentication.DefaultUrl);
            }
        }
    }
}