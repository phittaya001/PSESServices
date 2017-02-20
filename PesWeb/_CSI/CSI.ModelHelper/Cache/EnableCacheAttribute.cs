using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ModelHelper.Cache
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class EnableCacheAttribute : Attribute
    {
        public CacheBehavior Behavior;
        public string[] Tags;
        public EnableCacheAttribute(params string[] tags)
        {
            Behavior = CacheBehavior.Clone;
            Tags = tags;
        }
        public EnableCacheAttribute(CacheBehavior behavior, params string[] tags)
        {
            Behavior = behavior;
            Tags = tags;
        }
    }

    public enum CacheBehavior
    {
        Clone,
        Singleton
    }
}
