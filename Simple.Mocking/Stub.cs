using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public sealed class Stub
    {
        public static T Interface<T>()
        {
            return CreateStubFromMock(Mock.Interface<T>);
        }

        public static T Delegate<T>()
        {
            return CreateStubFromMock(Mock.Delegate<T>);
        }


        public static ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression)
        {
            return Expect.WithHigherPrecedence.MethodCall(methodCallExpression);
        }

        public static ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression)
        {
            return Expect.WithHigherPrecedence.PropertyGet(propertyExpression);
        }


        static T CreateStubFromMock<T>(Func<T> createMock)
        {
            var target = createMock();

            Expect.AnyInvocationOn(target).
                SetsOutOrRefParameters(StubValue.ForType).
                Returns(StubValue.ForType);

            return target;
        }


    }


}
