using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSI.Common.Diagnostics
{
    public class ExecutionIdentity : IDisposable
    {
        [ThreadStatic]
        public static Stack<ExecutionIdentityData> CallStack;
        public ExecutionIdentity()
        {
            Initialize(null);
        }
        public ExecutionIdentity(object customData)
        {
            Initialize(customData);
        }
        private void Initialize(object customData)
        {
            if (null == CallStack)
                CallStack = new Stack<Diagnostics.ExecutionIdentityData>();

            var method = new StackFrame(2).GetMethod();
            CallStack.Push(new ExecutionIdentityData
            {
                ID = Guid.NewGuid().ToString(),
                FullClassName = method.DeclaringType.FullName,
                MethodName = method.Name,
                CustomData = customData,
            });
        }
        public void Dispose()
        {
            CallStack.Pop();
            if (CallStack.Count == 0)
                CallStack = null;
        }
        public static ExecutionIdentityData Current { get { return CallStack?.Peek(); } }
    }

    public class ExecutionIdentityData
    {
        public string ID { get; set; }
        public string FullClassName { get; set; }
        public string MethodName { get; set; }
        public object CustomData { get; set; }
    }
}
