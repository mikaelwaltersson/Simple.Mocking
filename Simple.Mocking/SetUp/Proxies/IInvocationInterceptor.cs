using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	public interface IInvocationInterceptor
	{
		void OnInvocation(IInvocation invocation);
	}
}