using System;

namespace CSI.Common.Configuration
{
    public interface IUserPreference
    {
        string AppName { get; }
        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);
        void Load();
        void Save();
        void SetValue<T>(string key, T value);
        object this[string key] { get; set; }
    }
}
