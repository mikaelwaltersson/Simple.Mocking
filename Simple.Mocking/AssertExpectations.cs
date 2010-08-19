using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
	public class AssertExpectations
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

		public static IAssertExpectationsIsMetForCallTo IsMetForCallTo
		{
			get { return AssertExpectationsIsMetForCallTo.Instance; }
		}

		class AssertExpectationsIsMetForCallTo : IAssertExpectationsIsMetForCallTo
		{
			public static readonly IAssertExpectationsIsMetForCallTo Instance = new AssertExpectationsIsMetForCallTo();

			public IAssertExpectationsIsMetForCallTo MethodCall(Expression<Action> methodCallExpression)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForMethodCall(methodCallExpression));
			}

			public IAssertExpectationsIsMetForCallTo MethodCall<T>(Expression<Func<T>> methodCallExpression)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForMethodCall(methodCallExpression));
			}

			public IAssertExpectationsIsMetForCallTo PropertyGet<T>(Expression<Func<T>> propertyExpression)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForPropertyGet(propertyExpression));
			}

			public IAssertExpectationsIsMetForCallTo PropertySet<T>(Expression<Func<T>> propertyExpression, T value)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForPropertySet(propertyExpression, value));
			}

			public IAssertExpectationsIsMetForCallTo EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForEventAdd(target, eventName, handler));
			}

			public IAssertExpectationsIsMetForCallTo EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler)
			{
				return AssertMatchingInvocationWasCalled(InvocationMatcher.ForEventRemove(target, eventName, handler));
			}

			IAssertExpectationsIsMetForCallTo AssertMatchingInvocationWasCalled(InvocationMatcher invocationMatcher)
			{
				var invocationHistory = MockInvocationInterceptor.GetFromTarget(invocationMatcher.Target).ExpectationScope.InvocationHistory;

				if (!invocationHistory.ExpectedInvocations.Any(invocationMatcher.Matches))
					throw new ExpectationsException(invocationHistory, "Expected invocation '{0}' was not made, invocations:", invocationMatcher);

				return this;
			}
		}
			
	}
}
