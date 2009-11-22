using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
	class MockInvocationInterceptor : IInvocationInterceptor
	{
		IExpectationScope expectationScope;
		

		public MockInvocationInterceptor(IExpectationScope expectationScope)
		{
			if (expectationScope == null)
				throw new ArgumentNullException("expectationScope");

			this.expectationScope = expectationScope;
		}

		public IExpectationScope ExpectationScope
		{
			get { return expectationScope; }
		}


		public void OnInvocation(IInvocation invocation)
		{
			if (invocation == null)
				throw new ArgumentNullException("invocation");

			if (expectationScope.TryMeet(invocation))
				return;

			if (invocation.Method.DeclaringType == typeof(object))
			{			
				invocation.ReturnValue = invocation.Method.Invoke(invocation.Target.BaseObject, invocation.ParameterValues.ToArray());
				return;
			}

			throw new ExpectationsException(expectationScope, "Unexpected invocation '{0}', expected:", invocation);
		}

		public void AddExpectation(IExpectation expectation)
		{
			if (expectation == null)
				throw new ArgumentNullException("expectation");

			expectationScope.Add(expectation);
		}
    

		public static MockInvocationInterceptor GetFromTarget(object target)
		{
			if (target is Delegate)
				target = ((Delegate)target).Target;

			var proxy = target as IProxy;

			if (proxy == null)
				throw new ArgumentException("Target is not an proxy", "target");


			var mockInvocationInterceptor = proxy.InvocationInterceptor as MockInvocationInterceptor;

			if (mockInvocationInterceptor == null)
				throw new ArgumentException("Target proxy is not an mock object", "target");

			return mockInvocationInterceptor;
		}
	}

	
}
