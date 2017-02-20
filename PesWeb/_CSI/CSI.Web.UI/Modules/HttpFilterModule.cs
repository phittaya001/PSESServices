using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using CSI.CastleWindsorHelper;
using CSI.Security.Authorization;
using CSI.Security;
using CSI.Web.UI.Common;

namespace CSI.Web.UI.Modules
{
    public class HttpFilterModule : IHttpModule
    {
        public static bool EncryptUrlQuery = false;
        public static bool SingleSessionAuthen = false;
        public static IPreRenderPage PreRenderPage = null;
        private static string KeySault = "";

        private IAuthorization authorSvc;

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            KeySault = Guid.NewGuid().ToString();
            if (null == PreRenderPage)
                PreRenderPage = new PreRenderPageNormal();

            authorSvc = ServiceContainer.GetService<IAuthorization>();
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            if (EncryptUrlQuery)
                context.BeginRequest += OnBeginRequest;
        }
        private void OnBeginRequest(object sender, EventArgs args)
        {
            HttpContext context = HttpContext.Current;
            string query = context.Request.RawUrl;

            const string QParamName = "QPARAM";

            if (context.Request.Url.OriginalString.Contains(".aspx") && query.Contains("?") && context.Request.Cookies.AllKeys.Contains(".ASPXAUTH"))
            {
                var param = query.Split(new char[] { '?' }, 2);
                if (param.Length == 2)
                {
                    if (query.Contains(QParamName))
                    {
                        var encrypted = context.Request.QueryString[QParamName];
                        context.RewritePath(param[0], string.Empty, SecurityModelCrypto.Decrypt(encrypted));
                    }
                    else if (0 == string.Compare(context.Request.HttpMethod, "GET", true))
                    {
                        string newUrl = string.Format("{0}?{1}={2}", param[0], QParamName, SecurityModelCrypto.Encrypt(param[1]));
                        context.Response.Redirect(newUrl);
                    }
                }
            }
        }
        private void OnPreRequestHandlerExecute(object sender, EventArgs args)
        {
            HttpApplication app = sender as HttpApplication;
            if (app != null)
            {
                Page page = app.Context.Handler as Page;
                string url = app.Request.Url.AbsolutePath;
                if (false == url.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
                    url += ".aspx";

                var allowedAnonymous = authorSvc.GetAllowAnonymousResources();

                if (page != null &&
                    false == allowedAnonymous.Contains(url, StringComparer.InvariantCultureIgnoreCase) &&
                    0 != string.Compare(FormsAuthentication.LoginUrl, url, true))
                {
                    bool unauthenticated = app.User.Identity.IsAuthenticated == false;
                    if (unauthenticated)
                    {
                        page.Session.Abandon();
                        app.AbortResponse(401);
                    }
                    else
                    {
                        string authenName = page.User.Identity.Name;
                        string sessionOnwer = page.Session[Const.SessionOwnerKey] as string ?? string.Empty;
                        if (SingleSessionAuthen && 0 != string.Compare(authenName, sessionOnwer, true))
                            app.AbortResponse(401);
                        else
                        {
                            var deniedPages = authorSvc.GetDeniedResources(authenName);
                            string appUrl = app.Request.AppRelativeCurrentExecutionFilePath;
                            if ((deniedPages.Contains(url, StringComparer.InvariantCultureIgnoreCase) ||
                                 deniedPages.Contains(appUrl, StringComparer.InvariantCultureIgnoreCase)))
                                app.AbortResponse(403);
                            else
                            {
                                page.InitComplete += (s, a) => { PreRenderPage.OnInitComplete(authorSvc, s, a); };
                                page.PreRender += (s, a) => { PreRenderPage.OnPreRender(authorSvc, s, a); };
                                page.Init += (s, a) =>
                                {
                                    Page p = s as Page;
                                    if (null != p)
                                        p.ViewStateUserKey = KeySault + p.Session.SessionID;
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
