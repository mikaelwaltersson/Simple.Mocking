using Simple.Mocking.Asserts;
using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public static class AssertInvocationsWasMade
    {
        public static void MatchingExpectationsFor(object target)
		{
			var mockInvocationInterceptor = MockInvocationInterceptor.GetFromTarget(target);

            AssertExpectationScopeIsMet(mockInvocationInterceptor.ExpectationScope);
		}

        public static void MatchingExpectationsFor(ExpectationScope expectationScope) =>
		    AssertExpectationScopeIsMet(expectationScope);


        static void AssertExpectationScopeIsMet(IExpectationScope expectationScope)
        {
            if (!expectationScope.HasBeenMet)
                throw new ExpectationsException(expectationScope, "All expectations has not been met, expected:");
        }

        static IAssertInvocations BeginAssertions() => new AssertInvocations(null!);

        public static IAssertInvocationFor None => BeginAssertions().None;
    
        public static IAssertInvocationFor Once => BeginAssertions().Once;

        public static IAssertInvocationFor AtLeastOnce => BeginAssertions().AtLeastOnce;

        public static IAssertInvocationFor AtMostOnce => BeginAssertions().AtMostOnce;

        public static IAssertInvocationFor Exactly(int times) => BeginAssertions().Exactly(times);

        public static IAssertInvocationFor AtLeast(int times) => BeginAssertions().AtLeast(times);

        public static IAssertInvocationFor AtMost(int times) => BeginAssertions().AtMost(times);

        public static IAssertInvocationFor Between(int fromInclusive, int toInclusive) => BeginAssertions().Between(fromInclusive, toInclusive);
    }
}
