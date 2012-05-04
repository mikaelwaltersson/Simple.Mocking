using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.Asserts;
using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public static class AssertInvocationsWasMade
    {
        public static void MatchingExpecationsFor(object target)
		{
			var mockInvocationInterceptor = MockInvocationInterceptor.GetFromTarget(target);

            AssertExpectationScopeIsMet(mockInvocationInterceptor.ExpectationScope);
		}

        public static void MatchingExpecationsFor(ExpectationScope expectationScope)
		{
            AssertExpectationScopeIsMet(expectationScope);
		}


        static void AssertExpectationScopeIsMet(IExpectationScope expectationScope)
        {
            if (!expectationScope.HasBeenMet)
                throw new ExpectationsException(expectationScope, "All expectations has not been met, expected:");
        }


        static IAssertInvocations BeginAssertions()
        {
            return new AssertInvocations(null);
        }

        public static IAssertInvocationFor None
        {
            get { return BeginAssertions().None; }
        }

        public static IAssertInvocationFor Once
        {
            get { return BeginAssertions().Once; }
        }

        public static IAssertInvocationFor AtLeastOnce
        {
            get { return BeginAssertions().AtLeastOnce; }
        }

        public static IAssertInvocationFor AtMostOnce
        {
            get { return BeginAssertions().AtMostOnce; }
        }

        public static IAssertInvocationFor Exactly(int times)
        {
            return BeginAssertions().Exactly(times);
        }

        public static IAssertInvocationFor AtLeast(int times)
        {
            return BeginAssertions().AtLeast(times);
        }

        public static IAssertInvocationFor AtMost(int times)
        {
            return BeginAssertions().AtMost(times);
        }

        public static IAssertInvocationFor Between(int fromInclusive, int toInclusive)
        {
            return BeginAssertions().Between(fromInclusive, toInclusive);
        }


    }
}
