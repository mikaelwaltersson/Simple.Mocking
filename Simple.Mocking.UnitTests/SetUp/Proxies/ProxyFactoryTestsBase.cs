using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	public abstract class ProxyFactoryTestsBase
	{
		internal object baseObject;
		internal TestInvocationInterceptor invocationInterceptor;
		internal ProxyFactory proxyFactory;


		[SetUp]
		public void SetUp()
		{
			baseObject = new object();
			invocationInterceptor = new TestInvocationInterceptor();
			proxyFactory = new ProxyFactory();

			OnSetUp();
		}

		protected virtual void OnSetUp()
		{

		}



	}
}
