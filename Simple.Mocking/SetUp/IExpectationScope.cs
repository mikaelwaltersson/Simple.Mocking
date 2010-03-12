using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
	interface IExpectationScope : IExpectation
	{
		void Add(IExpectation expectation);

		void OnUnexpectedInvocation(IInvocation invocation);
	}
}