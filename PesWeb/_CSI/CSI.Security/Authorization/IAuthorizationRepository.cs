using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Security.Authorization
{
    public interface IAuthorizationRepository
    {
        List<ScreenItem> LoadScreenItem();
        List<UserPermissionMap> LoadPermissionUserMap();
        List<GroupPermissionMap> LoadPermissionGroupMap();
        Dictionary<string, List<string>> LoadUserGroup();
        Dictionary<string, List<RestrictedControlItem>> LoadRestrictedControlItem();
    }
    public enum AccessControlAction
    {
        None,
        Hide,
        Disable,
        ReadOnly,
    }
    public enum PermissionCode
    {
        View,
        Add,
        Edit,
        Delete,
    }
    public partial class ScreenItem
    {
        public string ScreenCode { get; set; }
        public string ScreenName { get; set; }
        public string MenuGroupCode { get; set; }
        public int ItemSequence { get; set; }
        public string AcResourceName { get; set; }
        public bool IsSingleton { get; set; }
        public bool AllowAnonymous { get; set; }
    }
    public partial class GroupPermissionMap
    {
        public string AcResourceName { get; set; }
        public string GroupCode { get; set; }
        public string PermissionCode { get; set; }
    }
    public partial class UserPermissionMap
    {
        public string AcResourceName { get; set; }
        public string UserCode { get; set; }
        public string LoginName { get; set; }
        public string PermissionCode { get; set; }
    }
    public partial class RestrictedControlItem
    {
        public string AcResourceName { get; set; }
        public string FullClassName { get; set; }
        public string ControlId { get; set; }
        public string PermissionCode { get; set; }
        public AccessControlAction ACA { get; set; }
    }
}
