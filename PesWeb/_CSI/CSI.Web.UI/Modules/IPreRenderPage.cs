using CSI.Security.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSI.Web.UI.Modules
{
    public interface IPreRenderPage
    {
        void OnPreRender(IAuthorization authorizeSvc, object sender, EventArgs args);
        void OnInitComplete(IAuthorization authorizeSvc, object sender, EventArgs args);
    }
}