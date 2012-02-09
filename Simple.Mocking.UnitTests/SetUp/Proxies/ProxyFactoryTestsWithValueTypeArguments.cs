using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithValueTypeArguments : ProxyFactoryTests<ProxyFactoryTestsWithValueTypeArguments.MyValueType>
	{
		protected override void OnSetUp()
		{
			expectedInput = new MyValueType(1, 2, 3);
			expectedInputIndex = new MyValueType(12, 12, 12);
			expectedOutput = new MyValueType(3, 2, 1);
		}

		public class MyValueType
		{
			int i, j, k;

			public MyValueType(int i, int j, int k)
			{
				this.i = i;
				this.j = j;
				this.k = k;
			}
		}

	}
}