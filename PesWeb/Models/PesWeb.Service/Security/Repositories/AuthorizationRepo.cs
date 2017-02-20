using System;
using System.Collections.Generic;
using System.Linq;
using CSI.ModelHelper.Cache;
using CSI.Security.Authorization;
using PesWeb.Service.Common;

namespace PesWeb.Service.Security.Repositories
{
    public class AuthorizationRepo : IAuthorizationRepository
    {
        [EnableCache(CacheBehavior.Singleton, CacheTags.ScreenItem)]
        public virtual List<ScreenItem> LoadScreenItem()
        {
            using (SecurityEntities db = new SecurityEntities())
                return db.tbs_ScreenItem
                    .Select(a => new ScreenItem
                    {
                        AcResourceName = a.AcResourceName,
                        ItemSequence = a.ItemSequence,
                        MenuGroupCode = a.ManuGroupCode,
                        ScreenCode = a.ScreenCode,
                        ScreenName = a.ScreenName,
                        IsSingleton = a.IsSingleton ?? false,
                        AllowAnonymous = a.AllowAnonymous ?? false,
                    })
                    .ToList();
        }
        [EnableCache(CacheBehavior.Singleton, CacheTags.PermissionMap)]
        public virtual List<GroupPermissionMap> LoadPermissionGroupMap()
        {
            using (SecurityEntities db = new SecurityEntities())
                return db.tbs_PermissionGroupMap
                    .Select(a => new GroupPermissionMap
                    {
                        AcResourceName = a.AcResourceName,
                        GroupCode = a.GroupCode,
                        PermissionCode = a.PermissionCode,
                    })
                    .ToList();
        }
        [EnableCache(CacheBehavior.Singleton, CacheTags.PermissionMap, CacheTags.User)]
        public virtual List<UserPermissionMap> LoadPermissionUserMap()
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                var qry = from p in db.tbs_PermissionUserMap
                          join u in db.tbs_User on p.UserCode equals u.UserCode
                          select new UserPermissionMap
                          {
                              AcResourceName = p.AcResourceName,
                              UserCode = p.UserCode,
                              LoginName = u.LoginName,
                              PermissionCode = p.PermissionCode,
                          };
                return qry.ToList();
            }
        }
        [EnableCache(CacheBehavior.Singleton, CacheTags.UserGroup)]
        public virtual Dictionary<string, List<string>> LoadUserGroup()
        {
            using (SecurityEntities db = new SecurityEntities())
                return db.tbs_UserGroup
                    .GroupBy(a => a.UserCode)
                    .ToDictionary(k => k.Key, v => v.Select(a => a.GroupCode).ToList());
        }
        [EnableCache(CacheBehavior.Singleton, CacheTags.RestrictedControlItem)]
        public virtual Dictionary<string, List<RestrictedControlItem>> LoadRestrictedControlItem()
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                string disable = AccessControlAction.Disable.ToString();
                string hide = AccessControlAction.Hide.ToString();
                string readOnly = AccessControlAction.ReadOnly.ToString();
                return db.tbs_RestrictControlItem
                    .Select(a => new RestrictedControlItem
                    {
                        ACA = 0 == string.Compare(disable, a.AccessControlAction, true) ? AccessControlAction.Disable :
                              0 == string.Compare(hide, a.AccessControlAction, true) ? AccessControlAction.Hide :
                              0 == string.Compare(readOnly, a.AccessControlAction, true) ? AccessControlAction.ReadOnly :
                              AccessControlAction.None,
                        AcResourceName = a.AcResourceName,
                        ControlId = a.ControlId,
                        FullClassName = a.FullClassName,
                        PermissionCode = a.PermissionCode,
                    })
                    .GroupBy(a => a.FullClassName)
                    .ToDictionary(k => k.Key, v => v.ToList());
            }
        }
    }
}
