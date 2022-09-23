using System;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
    interface IExpectation
	{
		bool TryMeet(IInvocation invocation, out Action? action);

		bool HasBeenMet { get; }
	}
}