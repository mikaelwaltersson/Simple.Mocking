using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking.Asserts
{
    class AssertInvocationFor : IAssertInvocationFor
    {
        MatchedInvocations previousMatch;
        NumberOfInvocationsConstraint numberOfInvocationsConstraint;
        


        public AssertInvocationFor(MatchedInvocations previousMatch, NumberOfInvocationsConstraint numberOfInvocationsConstraint)
        {
            this.previousMatch = previousMatch;
            this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
        }

        public IAssertInvocations ForMethodCall(Expression<Action> methodCallExpression)
        {
            return Assert(InvocationMatcher.ForMethodCall(methodCallExpression));
        }

        public IAssertInvocations ForMethodCall<T>(Expression<Func<T>> methodCallExpression)
        {
            return Assert(InvocationMatcher.ForMethodCall(methodCallExpression));
        }

        public IAssertInvocations ForPropertyGet<T>(Expression<Func<T>> propertyExpression)
        {
            return Assert(InvocationMatcher.ForPropertyGet(propertyExpression));
        }

        public IAssertInvocations ForPropertySet<T>(Expression<Func<T>> propertyExpression, T value)
        {
            return Assert(InvocationMatcher.ForPropertySet(propertyExpression, value));
        }

        public IAssertInvocations ForEventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler)
        {
            return Assert(InvocationMatcher.ForEventAdd(target, eventName, handler));
        }

        public IAssertInvocations ForEventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler)
        {
            return Assert(InvocationMatcher.ForEventRemove(target, eventName, handler));
        }

        IAssertInvocations Assert(InvocationMatcher invocationMatcher)
        {
            var invocationHistory = MockInvocationInterceptor.GetFromTarget(invocationMatcher.Target).ExpectationScope.InvocationHistory;
            var matchingInvocations = invocationHistory.Invocations.Where(invocationMatcher.Matches).ToArray();

            if (!numberOfInvocationsConstraint.Matches(matchingInvocations.Length))
                throw new ExpectationsException(invocationHistory, "Wrong number of invocations for '{0}', expected {1} actual {2}:", invocationMatcher, numberOfInvocationsConstraint, matchingInvocations.Length);
       
            return new AssertInvocations(new MatchedInvocations(previousMatch, numberOfInvocationsConstraint, invocationMatcher, matchingInvocations));
        }
    }
}
