using CSI.Security.Authorization;
using CSI.Web.UI.Modules;
using CSI.Web.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace PesWeb.App.Modules
{
    public class PreRenderPageDevX : IPreRenderPage
    {
        public void OnInitComplete(IAuthorization authorizeSvc, object sender, EventArgs args)
        {
            //Page page = sender as Page;
            //var gridIDs = page.Session[page.GetAcControlsKey<ASPxGridView>()] as List<string>;
            //if (gridIDs == null)
            //    return;

            //gridIDs.ForEach(a =>
            //    {
            //        var ctrl = page.FindControl(a) as ASPxGridView;
            //        if (null != ctrl)
            //            ctrl.CustomButtonInitialize += OnGridViewCustomButtonInitialize;
            //    });
        }
        public void OnPreRender(IAuthorization authorizeSvc, object sender, EventArgs args)
        {
            Page page = sender as Page;
            if (page != null)
            {
                var className = page.GetType().BaseType.FullName;
                var allCtrls = page.GetAllControls().Where(a => string.IsNullOrEmpty(a.ID) == false);
                var deniedUrls = authorizeSvc.GetDeniedMenuItems(page.User.Identity.Name);
                var restricted = authorizeSvc.GetRestrictedControls(className, page.User.Identity.Name);

                //var menu = allCtrls.Where(a => a.GetType() == typeof(ASPxMenu)).Select(a => a as ASPxMenu).ToList();
                //foreach (var m in menu)
                //{
                //    var items = m.GetAllMenuItems();
                //    items.Where(a => deniedUrls.Contains(a.NavigateUrl, StringComparer.InvariantCultureIgnoreCase) && a.HasChildren == false).ToList()
                //        .ForEach(a => a.Visible = false);
                //    items.Where(a => a.HasVisibleChildren == false && a.HasChildren == true).ToList()
                //        .ForEach(a => a.Visible = false);
                //}
                if (restricted.Count > 0)
                {
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

                    //var grids = allCtrls
                    //    .Where(a => a.GetType() == typeof(ASPxGridView))
                    //    .Select(a => a as ASPxGridView)
                    //    .ToList();

                    //page.Session[page.GetAcControlsKey<Control>()] = ctrls;
                    //if (grids.Count > 0)
                    //{
                    //    page.Session[page.GetAcControlsKey<ASPxGridView>()] = grids.Select(a => a.UniqueID).ToList();
                    //    grids.ForEach(a => a.CustomButtonInitialize += OnGridViewCustomButtonInitialize);
                    //}
                }
            }
        }
        //protected void OnGridViewCustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        //{
        //    if (e.VisibleIndex == -1)
        //        return;

        //    ASPxGridView grid = sender as ASPxGridView;
        //    var ctrls = grid.Page.Session[grid.Page.GetAcControlsKey<Control>()] as Dictionary<string, AccessControlAction>;
        //    if (ctrls != null)
        //    {
        //        if (ctrls.ContainsKey(e.ButtonID))
        //        {
        //            if (ctrls[e.ButtonID] == AccessControlAction.Hide)
        //                e.Visible = DefaultBoolean.False;
        //            else if (ctrls[e.ButtonID] == AccessControlAction.Disable)
        //                e.Enabled = false;
        //        }
        //    }
        //}
    }

    internal static class DevXPageExtension
    {
        //public static List<DevExpress.Web.MenuItem> GetAllMenuItems(this ASPxMenu menu)
        //{
        //    List<DevExpress.Web.MenuItem> items = new List<DevExpress.Web.MenuItem>();
        //    Queue<DevExpress.Web.MenuItem> queue = new Queue<DevExpress.Web.MenuItem>();

        //    foreach (DevExpress.Web.MenuItem item in menu.Items)
        //    {
        //        queue.Enqueue(item);
        //        do
        //        {
        //            DevExpress.Web.MenuItem subItem = queue.Dequeue();
        //            items.Add(subItem);
        //            foreach (DevExpress.Web.MenuItem m in subItem.Items)
        //                queue.Enqueue(m);
        //        } while (queue.Count > 0);
        //    }
        //    return items;
        //}
    }
}