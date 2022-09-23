using System;
using System.Collections.Generic;
using System.Reflection;

namespace Simple.Mocking.SetUp.Proxies
{
    public interface IInvocation
	{
		IProxy Target { get; }

		MethodInfo Method { get; }

		IList<Type>? GenericArguments { get; }

		IList<object?> ParameterValues { get; }

		object? ReturnValue { set; }

	    long InvocationOrder { get; }
	}
}