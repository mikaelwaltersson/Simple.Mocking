using System;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyTypeCacheTests
	{
		[Test]
		public void CantInvokeGetProxyTypeWithNullArgument()
		{	
		    Assert.Throws<ArgumentNullException>(() => new ProxyTypeCache().GetProxyType(null!, t => t));
            Assert.Throws<ArgumentNullException>(() => new ProxyTypeCache().GetProxyType(typeof(IMyInterface), null!));
		}		

		[Test]
		public void CreateTypeIsOnlyInvokedOnceOnSuccess()
		{
			int invocationCount = 0;

			Func<Type, Type> createTypeDelegate =
				type =>
				{
					invocationCount++;
					return typeof(MyProxyClass);
				};

			var cache = new ProxyTypeCache();


			for (int i = 0; i < 2; i++)
				Assert.AreEqual(typeof(MyProxyClass), cache.GetProxyType(typeof(IMyInterface), createTypeDelegate));
			
			Assert.AreEqual(1, invocationCount);
		}

		[Test]
		public void CreateTypeIsOnlyInvokedOnceOnFailure()
		{
			int invocationCount = 0;
			var createTypeException = new Exception();

			Func<Type, Type> createTypeDelegate =
				type =>
				{
					invocationCount++;
					throw createTypeException;
				};

			var cache = new ProxyTypeCache();

			for (var i = 0; i < 2; i++)
			{
                var ex = Assert.Throws<InvalidOperationException>(() => cache.GetProxyType(typeof(IMyInterface), createTypeDelegate))!;
                Assert.AreSame(createTypeException, ex.InnerException);
			}

			Assert.AreEqual(1, invocationCount);			
		}

		interface IMyInterface
		{			
		}

		class MyProxyClass
		{			
		}
	}
}