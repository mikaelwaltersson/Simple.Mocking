using System;
using System.Linq.Expressions;

namespace Simple.Mocking.Syntax
{
    [Obsolete("Use AssertInvocationsWasMade instead (will be removed in next version)")]
	public interface IAssertExpectationsIsMetForCallTo
	{
		IAssertExpectationsIsMetForCallTo MethodCall(Expression<Action> methodCallExpression);

		IAssertExpectationsIsMetForCallTo MethodCall<T>(Expression<Func<T>> methodCallExpression);

		IAssertExpectationsIsMetForCallTo PropertyGet<T>(Expression<Func<T>> propertyExpression);

		IAssertExpectationsIsMetForCallTo PropertySet<T>(Expression<Func<T>> propertyExpression, T value);

		IAssertExpectationsIsMetForCallTo EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler);

		IAssertExpectationsIsMetForCallTo EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler);		
	}
}