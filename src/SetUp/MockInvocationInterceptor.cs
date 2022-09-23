using System;
using System.Linq;

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

		public IExpectationScope ExpectationScope => expectationScope;

		public void OnInvocation(IInvocation invocation)
		{
			if (invocation == null)
				throw new ArgumentNullException("invocation");

		    var wasMet =
		        expectationScope.TryMeet(invocation, out var action) ||
		        TryMeetDefaultObjectMethodInvocation(invocation, out action);

		    expectationScope.InvocationHistory.RegisterInvocation(invocation, wasMet);

            if (wasMet)
                action!();
            else
                throw new ExpectationsException(expectationScope, "Unexpected invocation '{0}', expected:", invocation);
		}

		static bool TryMeetDefaultObjectMethodInvocation(IInvocation invocation, out Action? action)
		{
			if (invocation.Method.DeclaringType == typeof(object))
			{
				action = () => invocation.ReturnValue = invocation.Method.Invoke(invocation.Target.BaseObject, invocation.ParameterValues.ToArray());
				return true;
			}

		    action = null;
			return false;
		}


		public void AddExpectation(IExpectation expectation, bool hasHigherPrecedence)
		{
			if (expectation == null)
				throw new ArgumentNullException("expectation");

			expectationScope.Add(expectation, hasHigherPrecedence);
		}
    

		public static MockInvocationInterceptor GetFromTarget(object? target)
		{
		    target = InvocationTarget.UnwrapDelegateTarget(target);

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
