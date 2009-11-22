using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	public interface IInvocation
	{
		IProxy Target { get; }

		MethodInfo Method { get; }

		IList<Type> GenericArguments { get; }

		IList<object> ParameterValues { get; }

		object ReturnValue { set; }
	}
}