using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.ModelHelper.Paging;

namespace PesWeb.Service.Security
{
    //public partial class SecurityEntities
    //{
    //    public SecurityEntities(string nameOrConnectionString)
    //        : base(nameOrConnectionString)
    //    {
    //    }
    //}

    public partial class tbs_Group : IPagingCriteria
    {
        public PagingParam PageParam { get; set; }
        public SortingParam[] SortParams { get; set; }
        public bool? IsActiveParam { get; set; }
        public static string GetSqlDelete(params string[] groupCodes)
        {
            var gCodes = string.Join(",", groupCodes.Select(a => string.Format("'{0}'", a)).ToArray());
            return string.Format("delete tbs_Group where GroupCode in ({0})", gCodes);
        }
    }
    public partial class tbs_User : IPagingCriteria
    {
        public PagingParam PageParam { get; set; }
        public SortingParam[] SortParams { get; set; }
        public bool? IsActiveParam { get; set; }
        public string GroupCodeParam { get; set; }
        public static string GetSqlDelete(params string[] userCodes)
        {
            var uCodes = string.Join(",", userCodes.Select(a => string.Format("'{0}'", a)).ToArray());
            return string.Format("delete tbs_User where userCode in ({0})", uCodes);
        }
    }
    public partial class tbs_UserGroup
    {
        public static string GetSqlDelete(string userCode, string groupCode)
        {
            return string.Format("delete tbs_UserGroup where userCode = '{0}' and groupCode = '{1}'"
                , userCode, groupCode);
        }
        public static string GetSqlDeleteByUser(params string[] userCodes)
        {
            var uCodes = string.Join(",", userCodes.Select(a => string.Format("'{0}'", a)).ToArray());
            return string.Format("delete tbs_UserGroup where userCode in ({0})", uCodes);
        }
        public static string GetSqlDeleteByGroup(params string[] groupCodes)
        {
            var gCodes = string.Join(",", groupCodes.Select(a => string.Format("'{0}'", a)).ToArray());
            return string.Format("delete tbs_UserGroup where groupCode in ({0})", gCodes);
        }
    }
    public partial class tbs_ScreenItem
    {
        public List<tbs_Permission> Permissions { get; set; }
    }
    public partial class tbs_PermissionUserMap
    {
        public static string GetSqlDelete(string acResourceName, string userCode, string permissionCode)
        {
            return string.Format("delete tbs_PermissionUserMap where acResourceName = '{0}' and userCode = '{1}' and permissionCode = '{2}'"
                , acResourceName, userCode, permissionCode);
        }
    }
    public partial class tbs_PermissionGroupMap
    {
        public static string GetSqlDelete(string acResourceName, string groupCode, string permissionCode)
        {
            return string.Format("delete tbs_PermissionGroupMap where acResourceName = '{0}' and groupCode = '{1}' and permissionCode = '{2}'"
                , acResourceName, groupCode, permissionCode);
        }
    }
}
