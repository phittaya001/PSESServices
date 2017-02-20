using CSI.CastleWindsorHelper;
using CSI.Security.Authentication;
using CSI.Security.Authorization;
using CSI.Web.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

namespace CSI.Web.UI.Common
{
    public static class WebFormExtension
    {
        public static void ApplyAccessControlAction(this Page page, AccessControlAction action, params Control[] ctrls)
        {
            string pageClassName = page.GetType().BaseType.FullName;
            IAuthorization svc = ServiceContainer.GetService<IAuthorization>();
            var acl = svc.GetRestrictedControls(pageClassName, page.User.Identity.Name)
                .Select(a => a.ControlId);

            // filter the restricted controls off.
            List<Control> targets = ctrls.Where(a => false == acl.Contains(a.ID)).ToList();

            PropertyInfo p;

            if (action == AccessControlAction.Hide)
                targets.ForEach(a => a.Visible = false);
            else if (action == AccessControlAction.Disable)
                targets.ForEach(a =>
                {
                    Type t = a.GetType();
                    p = t.GetProperty("Enabled", typeof(bool));
                    if (p != null)
                        p.SetValue(a, false);
                });
            else if (action == AccessControlAction.ReadOnly)
                targets.ForEach(a =>
                {
                    Type t = a.GetType();
                    p = t.GetProperty("ReadOnly", typeof(bool));
                    if (p != null)
                        p.SetValue(a, true);
                });
            else if (action == AccessControlAction.None)
                targets.ForEach(a =>
                {
                    a.Visible = true;
                    Type t = a.GetType();
                    p = t.GetProperty("Enabled", typeof(bool));
                    if (p != null)
                        p.SetValue(a, true);
                    p = t.GetProperty("ReadOnly", typeof(bool));
                    if (p != null)
                        p.SetValue(a, false);
                });
        }
        public static string GetAcControlsKey<T>(this Page page) where T : Control
        {
            return string.Format("{0}.{1}.AccessControlled.{2}"
                , page.User.Identity.Name
                , page.GetType().BaseType.FullName
                , typeof(T).Name);
        }
        public static List<Control> GetAllControls(this Page page)
        {
            List<Control> items = new List<Control>();
            Queue<Control> queue = new Queue<Control>();

            foreach (Control item in page.Controls)
            {
                queue.Enqueue(item);
                do
                {
                    Control subItem = queue.Dequeue();
                    items.Add(subItem);
                    foreach (Control c in subItem.Controls)
                        queue.Enqueue(c);
                } while (queue.Count > 0);
            }
            return items;
        }
        public static void RegisterSessionOnwer(this Page page, string ownerName)
        {
            page.Session[Const.SessionOwnerKey] = ownerName;
        }
        public static void AbortResponse(this HttpApplication app, int statusCode)
        {
            app.Response.Clear();
            app.Response.StatusCode = statusCode;
            app.Response.End();
        }
    }
}