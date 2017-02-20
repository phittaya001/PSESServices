using System;
using System.Collections.Generic;
using System.Linq;

namespace CSI.Security.Authorization
{
    public class DefaultAuthorization : IAuthorization
    {
        protected IAuthorizationRepository Repository { get; set; }
        public DefaultAuthorization(IAuthorizationRepository repository)
        {
            Repository = repository;
        }
        public virtual List<string> GetDeniedResources(string loginName)
        {
            var allowed = GetResourcesByPermission("*", loginName);
            var denied = Repository.LoadScreenItem()
                .Where(a => false == allowed.Contains(a.AcResourceName))
                .Select(a => a.AcResourceName)
                .ToList();

            return denied;
        }
        public List<string> GetAllowAnonymousResources()
        {
            return Repository.LoadScreenItem().Where(a => a.AllowAnonymous).Select(a => a.AcResourceName).ToList();
        }
        public virtual List<string> GetDeniedMenuItems(string loginName)
        {
            var allowed = GetResourcesByPermission(PermissionCode.View.ToString(), loginName);
            var allowedItems = Repository.LoadScreenItem()
                .Where(a => allowed.Contains(a.AcResourceName) || string.IsNullOrEmpty(a.AcResourceName))
                .ToList();

            var groups = allowedItems
                .Select(a => a.MenuGroupCode)
                .ToList();

            var visibleItems = allowedItems
                .Where(a => groups.Contains(a.ScreenCode) || false == string.IsNullOrEmpty(a.AcResourceName))
                .ToList();

            return visibleItems.Select(a => a.AcResourceName).ToList();
        }
        protected virtual List<string> GetResourcesByPermission(string permissionCode, string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
                return new List<string>();

            var groupMap = Repository.LoadUserGroup();

            List<string> groupCodes;
            if (false == groupMap.TryGetValue(loginName, out groupCodes))
                groupCodes = new List<string>();

            var permitGroup = Repository.LoadPermissionGroupMap()
                .Where(a => true == groupCodes.Contains(a.GroupCode, StringComparer.InvariantCultureIgnoreCase));

            var permitUser = Repository.LoadPermissionUserMap()
                .Where(a => 0 == string.Compare(a.LoginName, loginName, true));

            if (permissionCode != "*")
            {
                permitGroup = permitGroup.Where(a => 0 == string.Compare(a.PermissionCode, permissionCode, true));
                permitUser = permitUser.Where(a => 0 == string.Compare(a.PermissionCode, permissionCode, true));
            }

            var resources = permitUser.Select(a => a.AcResourceName)
                .Union(permitGroup.Select(a => a.AcResourceName))
                .Distinct()
                .ToList();

            return resources;
        }
        public virtual List<RestrictedControlItem> GetRestrictedControls(string fullClassName, string loginName)
        {
            var allRestricts = Repository.LoadRestrictedControlItem();
            List<RestrictedControlItem> restricts;
            if (false == allRestricts.TryGetValue(fullClassName, out restricts))
                return new List<RestrictedControlItem>();

            if (restricts.Count == 0)
                return new List<RestrictedControlItem>();

            var groupMap = Repository.LoadUserGroup();
            List<string> groupCodes;
            if (false == groupMap.TryGetValue(loginName, out groupCodes))
                groupCodes = new List<string>();

            var resourceName = restricts[0].AcResourceName;

            var allPermUser = Repository.LoadPermissionUserMap();
            var permUser = allPermUser
                .Where(a => 0 != string.Compare(a.LoginName, loginName, true) &&
                            0 == string.Compare(a.AcResourceName, resourceName, true));

            var allPermGroup = Repository.LoadPermissionGroupMap();
            var permGroup = allPermGroup
                .Where(a => false == groupCodes.Contains(a.GroupCode, StringComparer.InvariantCultureIgnoreCase) &&
                            0 == string.Compare(a.AcResourceName, resourceName, true));

            var permissionCodes = (from u in permUser
                                   join g in permGroup
                                   on u.PermissionCode equals g.PermissionCode
                                   select u.PermissionCode);

            // case link-button
            var allowedRcGroup = allPermGroup
                .Where(a => true == groupCodes.Contains(a.GroupCode, StringComparer.InvariantCultureIgnoreCase));

            var allowedRcUser = allPermUser
                .Where(a => 0 == string.Compare(a.LoginName, loginName, true));

            var allowedRc = allowedRcUser.Select(a => a.AcResourceName)
                .Union(allowedRcGroup.Select(a => a.AcResourceName));

            var denied = Repository.LoadScreenItem()
                .Where(a => false == allowedRc.Contains(a.AcResourceName))
                .Select(a => a.AcResourceName)
                .ToList();

            // if link-button, PermissionCode will be AcResourceName
            var result = restricts
                .Where(a => permissionCodes.Contains(a.PermissionCode) || denied.Contains(a.PermissionCode))
                .ToList();

            return result;
        }
    }
}
