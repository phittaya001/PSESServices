using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ModelHelper.Cache
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class FlushCacheAttribute : Attribute
    {
        public string[] Tags;
        public FlushCacheAttribute(params string[] tags)
        {
            Tags = tags;
        }
    }
}
