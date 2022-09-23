using System.Collections.Generic;
using System.Linq;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.Asserts
{
    class MatchedInvocations
    {
        MatchedInvocations? previousMatch;
        NumberOfInvocationsConstraint numberOfInvocationsConstraint;
        InvocationMatcher invocationMatcher;
        IInvocation[] matchingInvocations;

        public MatchedInvocations(MatchedInvocations? previousMatch, NumberOfInvocationsConstraint numberOfInvocationsConstraint, InvocationMatcher invocationMatcher, IInvocation[] matchingInvocations)
        {
            this.previousMatch = previousMatch;
            this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
            this.invocationMatcher = invocationMatcher;
            this.matchingInvocations = matchingInvocations;
        }

        public static void AssertInvocationsWasMadeInSpecifiedOrder(MatchedInvocations? match)
        {
            var matches = GetAllPreviousMatchesInOrder(match);
            var expectationScope = GetOrderedExpectationScope(matches);

            foreach (var invocation in GetAllMatchingInvocationsInOrder(matches))
            {
                if (!expectationScope.TryMeet(invocation, out var action))
                    throw new ExpectationsException(expectationScope, "Invocations was not made in specified order (first mismatch at invocation '{0}'):", invocation);
            }
        }

        static List<MatchedInvocations> GetAllPreviousMatchesInOrder(MatchedInvocations? match)
        {
            var allMatches = new List<MatchedInvocations>();

            while (match != null)
            {
                allMatches.Add(match);
                match = match.previousMatch;
            }

            allMatches.Reverse();
            return allMatches;
        }

        static IExpectationScope GetOrderedExpectationScope(IEnumerable<MatchedInvocations> matches)
        {
            var expectationScope = new ExpectationScope();

            using (expectationScope.BeginOrdered())
            {
                foreach (var match in matches)
                    AddExpectation(expectationScope, match.invocationMatcher, match.numberOfInvocationsConstraint);                  
            }

            return expectationScope;
        }

        static void AddExpectation(IExpectationScope expectationScope, IInvocationMatcher invocationMatcher, NumberOfInvocationsConstraint numberOfInvocationsConstraint) =>
            expectationScope.Add(new Expectation(invocationMatcher, numberOfInvocationsConstraint), false);

        static IEnumerable<IInvocation> GetAllMatchingInvocationsInOrder(IEnumerable<MatchedInvocations> matches) =>
            matches.SelectMany(match => match.matchingInvocations).OrderBy(invocation => invocation.InvocationOrder);
    }
}
