using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
	interface IExpectation
	{
		bool TryMeet(IInvocation invocation);

		bool HasBeenMet { get; }
	}

}