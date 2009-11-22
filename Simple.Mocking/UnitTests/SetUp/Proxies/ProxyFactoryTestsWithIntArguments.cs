using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithIntArguments : ProxyFactoryTests<int>
	{
		protected override void OnSetUp()
		{
			expectedInput = -123;
			expectedInputIndex = 6;
			expectedOutput = -321;
		}
	}
}