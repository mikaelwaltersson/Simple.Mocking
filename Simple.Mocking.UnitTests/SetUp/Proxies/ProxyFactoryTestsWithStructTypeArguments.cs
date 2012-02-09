using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithStructTypeArguments : ProxyFactoryTests<ProxyFactoryTestsWithStructTypeArguments.MyStructType>
	{
		protected override void OnSetUp()
		{
			expectedInput = new MyStructType(123, 345);
			expectedInputIndex = new MyStructType(0, 1);
			expectedOutput = new MyStructType(543, 321);
		}

		public struct MyStructType
		{
			int a, b;

			public MyStructType(int a, int b)
			{
				this.a = a;
				this.b = b;
			}
		}

	}
}