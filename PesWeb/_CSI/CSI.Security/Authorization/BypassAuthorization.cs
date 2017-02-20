using System;
using System.Collections.Generic;
using System.Linq;

namespace CSI.Security.Authorization
{
    public class BypassAuthorization : IAuthorization
    {
        public BypassAuthorization()
        {
        }

        public virtual List<string> GetDeniedResources(string loginName)
        {
            return new List<string>();
        }
        public List<string> GetAllowAnonymousResources()
        {
            return new List<string>();
        }
        public virtual List<RestrictedControlItem> GetRestrictedControls(string fullClassname, string loginName)
        {
            return new List<RestrictedControlItem>();
        }

        public virtual List<string> GetDeniedMenuItems(string loginName)
        {
            return new List<string>();
        }
    }
}
