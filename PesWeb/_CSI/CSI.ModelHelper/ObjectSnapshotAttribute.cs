using System;

namespace CSI.ModelHelper
{
    [Serializable]
    [Flags]
    public enum SnapshotFlags
    {
        None = 0,
        Default = 0,
        Ignore = 1,
        DomainData = 2,
        UiData = 4,
        NoCount = 8
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ObjectSnapshotAttribute : Attribute
    {
        public SnapshotFlags Flags { get; private set; }

        public ObjectSnapshotAttribute()
        {
            Flags = SnapshotFlags.Default;
        }

        public ObjectSnapshotAttribute(SnapshotFlags viewModelFlags)
        {
            Flags = viewModelFlags;
        }
    }
}
