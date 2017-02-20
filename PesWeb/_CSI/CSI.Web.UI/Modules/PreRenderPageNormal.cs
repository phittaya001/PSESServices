using CSI.Security.Authorization;
using CSI.Web.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CSI.Web.UI.Modules
{
    public class PreRenderPageNormal : IPreRenderPage
    {
        public void OnInitComplete(IAuthorization authorizeSvc, object sender, EventArgs args)
        {
        }
        public void OnPreRender(IAuthorization authorizeSvc, object sender, EventArgs args)
        {
            Page page = sender as Page;
            if (page != null)
            {
                var className = page.GetType().BaseType.FullName;
                var restricted = authorizeSvc.GetRestrictedControls(className, page.User.Identity.Name);

                if (restricted.Count > 0)
                {
                    var allCtrls = page.GetAllControls().Where(a => string.IsNullOrEmpty(a.ID) == false);
                    var restrictedDict = restricted.ToDictionary(k => k.ControlId, v => v);

                    allCtrls.Where(a => restrictedDict.ContainsKey(a.ID)).ToList()
                        .ForEach(a =>
                        {
                            if (restrictedDict[a.ID].ACA == AccessControlAction.Hide)
                                a.Visible = false;
                            else if (restrictedDict[a.ID].ACA == AccessControlAction.Disable)
                            {
                                Type t = a.GetType();
                                var p = t.GetProperty("Enabled", typeof(bool));
                                if (p != null)
                                    p.SetValue(a, false);
                            }
                            else if (restrictedDict[a.ID].ACA == AccessControlAction.ReadOnly)
                            {
                                Type t = a.GetType();
                                var p = t.GetProperty("ReadOnly", typeof(bool));
                                if (p != null)
                                    p.SetValue(a, true);
                            }
                        });
                }
            }
        }
    }
}