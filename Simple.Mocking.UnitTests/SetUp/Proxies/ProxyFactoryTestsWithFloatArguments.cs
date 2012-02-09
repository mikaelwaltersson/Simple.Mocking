using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithFloatArguments : ProxyFactoryTests<float>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0.123f;
			expectedInputIndex = 5;
			expectedOutput = 0.321f;
		}
	}
}