using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	class TestInvocationInterceptor : IInvocationInterceptor
	{
		Action<IInvocation> onInvocationHandler;
		int invocationCount;


		public Action<IInvocation> OnInvocationHandler
		{
			set { onInvocationHandler = value; }
		}

		public int InvocationCount
		{
			get { return invocationCount; }
		}


		public void OnInvocation(IInvocation invocation)
		{
			invocationCount++;
			onInvocationHandler(invocation);
		}
	}
}