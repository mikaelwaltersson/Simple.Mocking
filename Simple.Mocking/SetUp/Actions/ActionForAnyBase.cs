using System;
using System.Reflection;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
    abstract class ActionForAnyBase : IAction
	{
		Func<Type, object?> valueForType;

        protected ActionForAnyBase(Func<Type, object?> valueForType)
		{
			this.valueForType = valueForType;
		}        

	    public void ExecuteFor(IInvocation invocation) => 
			ExecuteFor(Invocation.GetNonGenericMethod(invocation), invocation);

	    protected abstract void ExecuteFor(MethodInfo nonGenericMethod, IInvocation invocation);

        protected object? GetValueForType(Type type) => valueForType(type);
	}
}