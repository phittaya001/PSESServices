using CSI.Security.Authorization;
using System.Collections.Generic;
using System.Linq;


namespace PesWeb.Service.Security
{
    public class NonCachedAuthorization : IAuthorization
    {
        public IAuthorizationRepository Repository { get; set; }

        public List<string> GetDeniedResources(string loginName)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                var allowed = GetResourcesByPermission("*", loginName);
                var denied = db.tbs_ScreenItem
                    .Where(a => false == allowed.Contains(a.AcResourceName))
                    .Select(a => a.AcResourceName)
                    .ToList();

                return denied;
            }
        }
        public List<string> GetAllowAnonymousResources()
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                var allowed = db.tbs_ScreenItem
                    .Where(a => a.AllowAnonymous ?? false)
                    .Select(a => a.AcResourceName)
                    .ToList();

                return allowed;
            }
        }
        protected List<string> GetResourcesByPermission(string permissionCode, string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
                return new List<string>();
            using (SecurityEntities db = new SecurityEntities())
            {
                var groupCodes = from ug in db.tbs_UserGroup
                                 join u in db.tbs_User on ug.UserCode equals u.UserCode
                                 where u.LoginName == loginName
                                 select ug.GroupCode;

                var permitGroup = db.tbs_PermissionGroupMap
                    .Where(a => true == groupCodes.Contains(a.GroupCode));

                var permitUser = from p in db.tbs_PermissionUserMap
                                 join u in db.tbs_User on p.UserCode equals u.UserCode
                                 where u.LoginName == loginName
                                 select p;

                if (permissionCode != "*")
                {
                    permitGroup = permitGroup.Where(a => a.PermissionCode == permissionCode);
                    permitUser = permitUser.Where(a => a.PermissionCode == permissionCode);
                }

                var resources = permitUser.Select(a => a.AcResourceName)
                    .Union(permitGroup.Select(a => a.AcResourceName))
                    .Distinct()
                    .ToList();

                return resources;
            }
        }

        public List<RestrictedControlItem> GetRestrictedControls(string fullClassname, string loginName)
        {
            string disable = AccessControlAction.Disable.ToString();
            string hide = AccessControlAction.Hide.ToString();
            string readOnly = AccessControlAction.ReadOnly.ToString();

            using (SecurityEntities db = new SecurityEntities())
            {
                var resourceNames = db.tbs_RestrictControlItem
                    .Where(a => a.FullClassName == fullClassname)
                    .Select(a => a.AcResourceName)
                    .ToList();

                if (resourceNames.Count == 0)
                    return new List<RestrictedControlItem>();

                var groupCodes = from ug in db.tbs_UserGroup
                                 join u in db.tbs_User on ug.UserCode equals u.UserCode
                                 where u.LoginName == loginName
                                 select ug.GroupCode;

                var resourceName = resourceNames[0];

                var permGroup = db.tbs_PermissionGroupMap
                    .Where(a => false == groupCodes.Contains(a.GroupCode) &&
                                a.AcResourceName == resourceName);

                var permUsers = from p in db.tbs_PermissionUserMap
                                join u in db.tbs_User on p.UserCode equals u.UserCode
                                where u.LoginName == loginName && p.AcResourceName == resourceName
                                select p;

                var permissionCodes = (from u in permUsers
                                       join g in permGroup
                                       on u.PermissionCode equals g.PermissionCode
                                       select u.PermissionCode);

                var restricts = db.tbs_RestrictControlItem
                    .Where(a => a.FullClassName == fullClassname &&
                                permissionCodes.Contains(a.PermissionCode))
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
                    .ToList();

                return restricts;
            }
        }

        public List<string> GetDeniedMenuItems(string loginName)
        {
            using (SecurityEntities db = new SecurityEntities())
            {
                var allowed = GetResourcesByPermission("View", loginName);
                var allowedItems = db.tbs_ScreenItem
                    .Where(a => allowed.Contains(a.AcResourceName) || string.IsNullOrEmpty(a.AcResourceName))
                    .Select(a => new ScreenItem
                    {
                        AcResourceName = a.AcResourceName,
                        ItemSequence = a.ItemSequence,
                        MenuGroupCode = a.ManuGroupCode,
                        ScreenCode = a.ScreenCode,
                        ScreenName = a.ScreenName,
                        IsSingleton = a.IsSingleton ?? false,
                    })
                    .ToList();

                var groups = allowedItems
                    .Select(a => a.MenuGroupCode)
                    .ToList();

                var visibleItems = allowedItems
                    .Where(a => groups.Contains(a.ScreenCode) || false == string.IsNullOrEmpty(a.AcResourceName))
                    .ToList();

                return visibleItems.Select(a => a.AcResourceName).ToList();
            }
        }
    }
}
