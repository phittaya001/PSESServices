using CSI.ModelHelper.Cache;
using CSI.Security.Authorization;
using PesWeb.Service.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace PesWeb.Service.Security
{
    public class PermissionMaintenanceSvc
    {
        public virtual List<tbs_ScreenItem> GetScreens(tbs_ScreenItem condition)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                var screens = db.tbs_ScreenItem
                    .Where(a => false == string.IsNullOrEmpty(a.AcResourceName));

                if (false == string.IsNullOrEmpty(condition.ScreenCode))
                    screens = screens.Where(a => a.ScreenCode.Contains(condition.ScreenCode));
                if (false == string.IsNullOrEmpty(condition.ScreenName))
                    screens = screens.Where(a => a.ScreenName.Contains(condition.ScreenName));

                var defaultPermis = db.tbs_Permission
                    .Where(a => a.PermissionCode == "View")
                    .FirstOrDefault();
                if (null == defaultPermis)
                    defaultPermis = new tbs_Permission { PermissionCode = PermissionCode.View.ToString() };

                var permis = (from a in db.tbs_ScreenPermission
                              join b in db.tbs_Permission
                              on a.PermissionCode equals b.PermissionCode
                              select new
                              {
                                  ScreenCode = a.ScreenCode,
                                  Permissions = b,
                              })
                              .GroupBy(a => a.ScreenCode)
                              .ToDictionary(k => k.Key, v => v.Select(a => a.Permissions).OrderBy(a => a.Sequence).ToList());

                var result = screens.ToList();
                result.ForEach(a =>
                {
                    List<tbs_Permission> v;
                    if (permis.TryGetValue(a.ScreenCode, out v))
                        a.Permissions = permis[a.ScreenCode];
                    else
                        a.Permissions = new List<tbs_Permission>() { defaultPermis };
                });

                return result;
            }
        }
        public virtual List<tbs_PermissionUserMap> GetScreenPermissionUserMap(string acResourceName)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                return db.tbs_PermissionUserMap
                    .Where(a => a.AcResourceName == acResourceName)
                    .ToList();
            }
        }
        public virtual List<tbs_PermissionGroupMap> GetScreenPermissionGroupMap(string acResourceName)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                return db.tbs_PermissionGroupMap
                    .Where(a => a.AcResourceName == acResourceName)
                    .ToList();
            }
        }
        [FlushCache(CacheTags.PermissionMap)]
        public virtual void UpdateScreenPermissions(UpdateScreenPermissionParam updates)
        {
            using (SecurityEntities db = new SecurityEntities())
            using (TransactionScope trans = new TransactionScope())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var g in updates.out_GroupPermissions)
                {
                    sb.Append(tbs_PermissionGroupMap.GetSqlDelete(g.AcResourceName, g.GroupCode, g.PermissionCode));
                    sb.AppendLine(";");
                }
                foreach (var g in updates.in_GroupPermissions)
                {
                    sb.Append(tbs_PermissionGroupMap.GetSqlDelete(g.AcResourceName, g.GroupCode, g.PermissionCode));
                    sb.AppendLine(";");
                }
                foreach (var u in updates.out_UserPermissions)
                {
                    sb.Append(tbs_PermissionUserMap.GetSqlDelete(u.AcResourceName, u.UserCode, u.PermissionCode));
                    sb.Append(";");
                }
                foreach (var u in updates.in_UserPermissions)
                {
                    sb.Append(tbs_PermissionUserMap.GetSqlDelete(u.AcResourceName, u.UserCode, u.PermissionCode));
                    sb.Append(";");
                }
                string sql = sb.ToString();
                if (false == string.IsNullOrEmpty(sql))
                    db.Database.ExecuteSqlCommand(sql);

                foreach (var g in updates.in_GroupPermissions)
                {
                    db.tbs_PermissionGroupMap.Add(
                        new tbs_PermissionGroupMap
                        {
                            AcResourceName = g.AcResourceName,
                            GroupCode = g.GroupCode,
                            PermissionCode = g.PermissionCode,
                        });
                    db.SaveChanges();
                }
                foreach (var g in updates.in_UserPermissions)
                {
                    db.tbs_PermissionUserMap.Add(
                        new tbs_PermissionUserMap
                        {
                            AcResourceName = g.AcResourceName,
                            UserCode = g.UserCode,
                            PermissionCode = g.PermissionCode,
                        });
                    db.SaveChanges();
                }

                trans.Complete();
            }
        }
    }

    public partial class UpdateScreenPermissionParam
    {
        public List<tbs_PermissionUserMap> in_UserPermissions { get; set; }
        public List<tbs_PermissionUserMap> out_UserPermissions { get; set; }
        public List<tbs_PermissionGroupMap> in_GroupPermissions { get; set; }
        public List<tbs_PermissionGroupMap> out_GroupPermissions { get; set; }
    }
}
