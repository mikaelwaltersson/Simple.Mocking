using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

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

		public static T Interface<T>()
		{
			return Interface<T>(new ExpectationScope());
		}

		public static T Interface<T>(ExpectationScope expectationScope)
		{
			return Create<T>(factory.CreateInterfaceProxy<T>, expectationScope);
		}

		public static T Delegate<T>()
		{
			return Delegate<T>(new ExpectationScope());
		}

		public static T Delegate<T>(ExpectationScope expectationScope)
		{
			return Create<T>(factory.CreateDelegateProxy<T>, expectationScope);
		}

		static T Create<T>(Func<object, MockInvocationInterceptor, T> factoryFunc, ExpectationScope expectationScope)
		{
			var mock = new Mock(MockName<T>.GetUniqueInScope(expectationScope));

			lock (factory)
			{
				return factoryFunc(mock, new MockInvocationInterceptor(expectationScope));
			}
		}

		public override string ToString()
		{
			return name;
		}
	}
}
