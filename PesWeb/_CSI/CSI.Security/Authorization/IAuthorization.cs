using System.Collections.Generic;
using System.ServiceModel;

namespace CSI.Security.Authorization
{
    [ServiceContract]
    public interface IAuthorization
    {
        List<string> GetDeniedResources(string loginName);
        List<string> GetAllowAnonymousResources();
        List<RestrictedControlItem> GetRestrictedControls(string fullClassName, string loginName);
        List<string> GetDeniedMenuItems(string loginName);
    }
    public interface IAuthorization<T> : IAuthorization
    {
        bool GetUserInformation(string username, ref T customData);
    }
}
