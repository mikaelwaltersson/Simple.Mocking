using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Simple.Mocking.Asserts;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    [Obsolete("Use AssertInvocationsWasMade instead (will be removed in next version)")]
	public class AssertExpectations
	{
		public static void IsMetFor(object target)
		{
			AssertInvocationsWasMade.MatchingExpectationsFor(target);
		}

		public static void IsMetFor(ExpectationScope expectationScope)
		{
            AssertInvocationsWasMade.MatchingExpectationsFor(expectationScope);
		}

		public static IAssertExpectationsIsMetForCallTo IsMetForCallTo
		{
            get { return AssertExpectationsIsMetForCallTo.Wrap(new AssertInvocations(null)); }
		}

        class AssertExpectationsIsMetForCallTo : IAssertExpectationsIsMetForCallTo
        {
            IAssertInvocationFor assertInvocation;

            public static IAssertExpectationsIsMetForCallTo Wrap(IAssertInvocations assertInvocations)
            {
                return new AssertExpectationsIsMetForCallTo { assertInvocation = assertInvocations.AtLeastOnce };
            }

            public IAssertExpectationsIsMetForCallTo MethodCall(Expression<Action> methodCallExpression)
            {
                return Wrap(assertInvocation.ForMethodCall(methodCallExpression));
            }

            public IAssertExpectationsIsMetForCallTo MethodCall<T>(Expression<Func<T>> methodCallExpression)
            {
                return Wrap(assertInvocation.ForMethodCall(methodCallExpression));
            }

            public IAssertExpectationsIsMetForCallTo PropertyGet<T>(Expression<Func<T>> propertyExpression)
            {
                return Wrap(assertInvocation.ForPropertyGet(propertyExpression));
            }

            public IAssertExpectationsIsMetForCallTo PropertySet<T>(Expression<Func<T>> propertyExpression, T value)
            {
                return Wrap(assertInvocation.ForPropertySet(propertyExpression, value));
            }

            public IAssertExpectationsIsMetForCallTo EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler)
            {
                return Wrap(assertInvocation.ForEventAdd(target, eventName, handler));
            }

            public IAssertExpectationsIsMetForCallTo EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler)
            {
                return Wrap(assertInvocation.ForEventRemove(target, eventName, handler));
            }
        }	
	}
}
