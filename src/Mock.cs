using System;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

#pragma warning disable 659

namespace Simple.Mocking
{
    public sealed class Mock
	{
		static readonly ProxyFactory factory = new ProxyFactory();

		string name;

		private Mock(string name)
		{
			this.name = name;
		}

		public static T Interface<T>() =>
			Interface<T>(new ExpectationScope());

		public static T Interface<T>(ExpectationScope expectationScope) =>
			Create<T>(factory.CreateInterfaceProxy<T>, expectationScope);

		public static T Delegate<T>() =>
			Delegate<T>(new ExpectationScope());

		public static T Delegate<T>(ExpectationScope expectationScope) =>
			Create<T>(factory.CreateDelegateProxy<T>, expectationScope);

		static T Create<T>(Func<object, MockInvocationInterceptor, T> factoryFunc, ExpectationScope expectationScope)
		{
			var mock = new Mock(MockName<T>.GetUniqueInScope(expectationScope));

			lock (factory)
			{
				return factoryFunc(mock, new MockInvocationInterceptor(expectationScope));
			}
		}

		public override string ToString() => name;

		public override bool Equals(object? obj) =>
			base.Equals(InvocationTarget.UnwrapProxyBaseObject(obj));
	}
}

