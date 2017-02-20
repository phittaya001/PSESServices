using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;
using CSI.CastleWindsorHelper;

namespace CSI.CastleWindsorHelper.Fake
{
    public sealed class FakeObjectInterceptor : IInterceptor
    {
        void IInterceptor.Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (NotImplementedException)
            {
                var outParams = invocation.Method.GetParameters();
                var valueGen = ServiceContainer.GetService<IFakeValueGenerator>();
                for (int i = 0; i < outParams.Length; i++)
                {
                    var p = outParams[i];
                    if (p.ParameterType.IsByRef)
                        invocation.Arguments[i] = valueGen.GenerateValuesFromType(p.ParameterType.GetElementType());
                }
                if (false == typeof(void).IsAssignableFrom(invocation.Method.ReturnType))
                    invocation.ReturnValue = valueGen.GenerateValuesFromType(invocation.Method.ReturnType);
            }
        }
    }
}
