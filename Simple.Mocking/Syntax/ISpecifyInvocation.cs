using System;
using System.Linq.Expressions;

namespace Simple.Mocking.Syntax
{
	public interface ISpecifyInvocation
	{
		ISpecifyAction MethodCall(Expression<Action> methodCallExpression);

		ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression);

		ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression);

		ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, T value);

		ISpecifyAction EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler);

		ISpecifyAction EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler);
	}
}