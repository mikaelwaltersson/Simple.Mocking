using System;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
	interface IExpectationScope : IExpectation
	{
		void Add(IExpectation expectation, bool hasHigherPrecedence);

		IInvocationHistory InvocationHistory { get; }
	}
}