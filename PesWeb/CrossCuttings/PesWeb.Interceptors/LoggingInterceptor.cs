using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Castle.DynamicProxy;
using CSI.Common.Exceptions;
using CSI.Common.Diagnostics;

namespace PesWeb.Interceptors
{
    public class LoggingInterceptor : IInterceptor
    {
        public static bool EnableExecutionLog { get; set; }
        void IInterceptor.Intercept(IInvocation invocation)
        {
            DateTime startTime = DateTime.Now;
            Exception outterException = null;
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                outterException = ex;
                var deepestException = ex.ExtractDataException();
                // write log here
                throw;
            }
            finally
            {
                if (EnableExecutionLog && ExecutionIdentity.CallStack != null)
                {
                    DateTime endTime = DateTime.Now;
                    // write log here
                }
            }
        }
        private string CreateCallingStatement(IInvocation invocation)
        {
            List<string> args = new List<string>();
            var pi = invocation.Method.GetParameters();
            for (int i = 0; i < pi.Length; i++)
                if (pi[i].ParameterType == typeof(string))
                    args.Add(string.Format("{0}: \"{1}\"", pi[i].Name, invocation.Arguments[i] ?? "null"));
                else
                    args.Add(string.Format("{0}: {1}", pi[i].Name, invocation.Arguments[i] ?? "null"));

            string statement = string.Format("{0}.{1}({2});"
                , invocation.MethodInvocationTarget.DeclaringType.FullName
                , invocation.MethodInvocationTarget.Name
                , string.Join(", ", args.ToArray()));

            return statement;
        }
    }
}
