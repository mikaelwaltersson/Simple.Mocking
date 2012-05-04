using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Simple.Mocking.SetUp.Proxies
{
	public sealed class InvocationFactory
	{
	    static long invocationOrderGenerator;

		MethodInfo method;
		

		InvocationFactory(MethodInfo method)
		{
			this.method = method;
		}



		public static InvocationFactory GetForMethod(MethodInfo method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return new InvocationFactory(method);
		}

		public static InvocationFactory GetForMethodTextRepresentation(Type declaringType, string methodTextRepresentation)
		{
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");

			if (methodTextRepresentation == null)
				throw new ArgumentNullException("methodTextRepresentation");

			var method = LookupMethodByTextRepresentation(declaringType, methodTextRepresentation);

			return new InvocationFactory(method);
		}

		
		internal static string GetTextRepresentationForMethod(MethodInfo methodInfo)
		{
			return Convert.ToString(methodInfo);
		}

		static MethodInfo LookupMethodByTextRepresentation(Type declaringType, string methodTextRepresentation)
		{
			foreach (var method in declaringType.GetMethods())
			{
				if (methodTextRepresentation == GetTextRepresentationForMethod(method))
					return method;
			}

			throw new MissingMethodException(string.Format("Method '{0}' is not declared by type '{1}'", methodTextRepresentation, declaringType));
		}


		internal Invocation CreateInvocation(IProxy target, Type[] genericArguments, object[] parameterValues, object returnValue)
		{
		    var invocationOrder = Interlocked.Increment(ref invocationOrderGenerator);

			return new Invocation(target, method, genericArguments, parameterValues, returnValue, invocationOrder);
		}

	}
}