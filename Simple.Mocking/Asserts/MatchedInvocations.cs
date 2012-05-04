using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.Asserts
{
    class MatchedInvocations
    {
        MatchedInvocations previousMatch;
        NumberOfInvocationsConstraint numberOfInvocationsConstraint;
        InvocationMatcher invocationMatcher;
        IInvocation[] matchingInvocations;



        public MatchedInvocations(MatchedInvocations previousMatch, NumberOfInvocationsConstraint numberOfInvocationsConstraint, InvocationMatcher invocationMatcher, IInvocation[] matchingInvocations)
        {
            this.previousMatch = previousMatch;
            this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
            this.invocationMatcher = invocationMatcher;
            this.matchingInvocations = matchingInvocations;
        }

        public void AssertInvocationsWasMadeInSpecifiedOrder()
        {
            var matches = GetAllPreviousMatchesInOrder(this);
            var expectationScope = GetOrderedExpectationScope(matches);

            foreach (var invocation in GetAllMatchingInvocationsInOrder(matches))
            {
                Action action;
                if (!expectationScope.TryMeet(invocation, out action))
                    throw new ExpectationsException(expectationScope, "Invocations was not made in specified order (first mismatch at invocation '{0}'):", invocation);
            }
        }

        static List<MatchedInvocations> GetAllPreviousMatchesInOrder(MatchedInvocations match)
        {
            var allMatches = new List<MatchedInvocations>();

            do
            {
                allMatches.Add(match);
                match = match.previousMatch;
            }
            while (match != null);

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

        static void AddExpectation(IExpectationScope expectationScope, IInvocationMatcher invocationMatcher, NumberOfInvocationsConstraint numberOfInvocationsConstraint)
        {
            expectationScope.Add(new Expectation(invocationMatcher, numberOfInvocationsConstraint), false);
        }


        IEnumerable<IInvocation> GetAllMatchingInvocationsInOrder(IEnumerable<MatchedInvocations> matches)
        {
            return matches.SelectMany(match => match.matchingInvocations).OrderBy(invocation => invocation.InvocationOrder);
        }



    }
}
