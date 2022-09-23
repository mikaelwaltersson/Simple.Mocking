using System;
using System.Linq.Expressions;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public sealed class Stub
    {
        public static T Interface<T>() where T : notnull =>
            CreateStubFromMock(Mock.Interface<T>);

        public static T Delegate<T>() where T : notnull =>
            CreateStubFromMock(Mock.Delegate<T>);

        public static ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression) =>
            Expect.WithHigherPrecedence.MethodCall(methodCallExpression);

        public static ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression) =>
            Expect.WithHigherPrecedence.PropertyGet(propertyExpression);

        static T CreateStubFromMock<T>(Func<T> createMock) where T : notnull
        {
            var target = createMock();

            Expect.AnyInvocationOn(target).
                SetsOutOrRefParameters(StubValue.ForType).
                Returns(StubValue.ForType);

            return target;
        }
    }
}
