using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Common.Configuration
{
    public abstract class UserPreferenceBase : IUserPreference
    {
        public string AppName { get; protected set; }
        protected Dictionary<string, object> DataTable = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        protected UserPreferenceBase(string appName)
        {
            AppName = appName;
        }
        public abstract void Load();
        public abstract void Save();
        public virtual object this[string key]
        {
            get
            {
                if (DataTable.ContainsKey(key))
                    return DataTable[key];
                return null;
            }
            set
            {
                lock (DataTable)
                {
                    DataTable[key] = value;
                }
            }
        }

        public virtual T GetValue<T>(string key)
        {
            object o = null;
            if (DataTable.ContainsKey(key))
                o = DataTable[key];
            T v = (T)Convert.ChangeType(o, typeof(T));

            SetValue(key, v);

            return v;
        }
        public virtual T GetValue<T>(string key, T defaultValue)
        {
            if (DataTable.ContainsKey(key))
                return GetValue<T>(key);
            else
                SetValue(key, defaultValue);

            return defaultValue;
        }
        public virtual void SetValue<T>(string key, T value)
        {
            lock (DataTable)
            {
                DataTable[key] = value;
            }
        }
    }
}
