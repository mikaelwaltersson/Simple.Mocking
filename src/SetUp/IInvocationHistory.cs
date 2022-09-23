using System.Collections.Generic;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
	interface IInvocationHistory
	{
		void RegisterInvocation(IInvocation invocation, bool wasExpected);

        IEnumerable<IInvocation> Invocations { get; }
		IEnumerable<IInvocation> UnexpectedInvocations { get; }
	}
}