using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Castle.DynamicProxy;

namespace CSI.ModelHelper.Cache
{
    public class CachedMethodInterceptor : IInterceptor
    {
        void IInterceptor.Intercept(IInvocation invocation)
        {
            var enableCacheAttr = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(EnableCacheAttribute), true);
            var resetCacheAttr = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(FlushCacheAttribute), true);

            if (resetCacheAttr.Length > 0)
            {
                FlushCacheAttribute attrib = resetCacheAttr[0] as FlushCacheAttribute;
                CacheContext.FlushByTags(attrib.Tags);
            }
            if (enableCacheAttr.Length > 0)
            {
                EnableCacheAttribute attrib = enableCacheAttr[0] as EnableCacheAttribute;
                using (StringWriter sw = new StringWriter())
                {
                    string methodKey = GetMethodKey(invocation);
                    sw.Write(methodKey);
                    foreach (var a in invocation.Arguments)
                    {
                        if (a == null)
                            sw.Write("null");
                        else
                        {
                            XmlSerializer xml = new XmlSerializer(a.GetType());
                            xml.Serialize(sw, a);
                        }
                    }
                    string key = sw.ToString();
                    if (CacheContext.ContainsKey(key))
                        invocation.ReturnValue = attrib.Behavior == CacheBehavior.Singleton ? CacheContext.GetValueDirect(key) : CacheContext.GetValue(key);
                    else
                    {
                        invocation.Proceed();
                        CacheContext.SetValue(key, invocation.ReturnValue, attrib.Tags);
                    }
                }
            }
            else
                invocation.Proceed();
        }
        private string GetMethodKey(IInvocation invocation)
        {
            return string.Format("{0}.{1}?", invocation.MethodInvocationTarget.DeclaringType.FullName, invocation.Method.Name);
        }
    }
}
