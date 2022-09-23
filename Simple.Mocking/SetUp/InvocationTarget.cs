using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
    static class InvocationTarget
    {
        public static object? UnwrapDelegateTarget(object? target)
        {
            if (target is Delegate)
                target = ((Delegate)target).Target;

            return target;
        }

        public static object? UnwrapProxyBaseObject(object? target)
        {
            if (target is IProxy)
                target = ((IProxy)target).BaseObject;

            return target;
        }

        public static object? UnwrapDelegateTargetAndProxyBaseObject(object? target) =>
            UnwrapProxyBaseObject(UnwrapDelegateTarget(target));

        public static bool IsDelegate(object? target)
        {
            if (target is Delegate)
                return true;

            if (target is IProxy)
                return ((IProxy)target).ProxiedType.IsDelegateType();

            return false;
        }
    }
}
