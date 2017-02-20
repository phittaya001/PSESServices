using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSI.Common.Configuration;

namespace PesWeb.Injectors.Preference
{
    public enum PrefKeys
    {
        LoginName,
        RememberLoginName,
    }
    public static class PreferenceExtension
    {
        private static Dictionary<PrefKeys, object> DefaultValues = new Dictionary<PrefKeys, object>()
        {
            {PrefKeys.LoginName, string.Empty},
            {PrefKeys.RememberLoginName, false},
        };
        public static T GetValue<T>(this IUserPreference pref, PrefKeys key)
        {
            string strKey = key.ToString();
            return pref.GetValue<T>(strKey, (T)DefaultValues[key]);
        }
        public static void SetValue<T>(this IUserPreference pref, PrefKeys key, T value)
        {
            string strKey = key.ToString();
            pref.SetValue(strKey, value);
        }
    }
}
