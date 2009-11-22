using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			try
			{
				new ProxyTypeCache().GetProxyType(null, t => t);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new ProxyTypeCache().GetProxyType(typeof(IMyInterface), null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
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

			for (int i = 0; i < 2; i++)
			{
				try
				{
					cache.GetProxyType(typeof(IMyInterface), createTypeDelegate);
					Assert.Fail();
				}
				catch (InvalidOperationException ex)
				{					
					Assert.AreSame(createTypeException, ex.InnerException);
				}
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