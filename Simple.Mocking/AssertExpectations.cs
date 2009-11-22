using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;

namespace Simple.Mocking
{
	public static class AssertExpectations
	{
		public static void IsMetFor(object target)
		{
			var mockInvocationInterceptor = MockInvocationInterceptor.GetFromTarget(target);

			AssertExpectationScopeIsMet(mockInvocationInterceptor.ExpectationScope);
		}

		public static void IsMetFor(ExpectationScope expectationScope)
		{
			AssertExpectationScopeIsMet(expectationScope);
		}

		static void AssertExpectationScopeIsMet(IExpectationScope expectationScope)
		{
			if (!expectationScope.HasBeenMet)
				throw new ExpectationsException(expectationScope, "All expectations has not been met, expected:");
		}
	}
}
