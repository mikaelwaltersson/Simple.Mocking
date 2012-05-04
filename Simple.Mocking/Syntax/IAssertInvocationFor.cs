using System;
using System.Linq.Expressions;

namespace Simple.Mocking.Syntax
{
    public interface IAssertInvocationFor
    {
        IAssertInvocations ForMethodCall(Expression<Action> methodCallExpression);

        IAssertInvocations ForMethodCall<T>(Expression<Func<T>> methodCallExpression);

        IAssertInvocations ForPropertyGet<T>(Expression<Func<T>> propertyExpression);

        IAssertInvocations ForPropertySet<T>(Expression<Func<T>> propertyExpression, T value);

        IAssertInvocations ForEventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler);

        IAssertInvocations ForEventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler);
    }
}