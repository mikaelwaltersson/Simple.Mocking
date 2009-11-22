using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithUnsignedLongArguments : ProxyFactoryTests<ulong>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0xF000000000001234;
			expectedInputIndex = 10;
			expectedOutput = 0xF000000000004321;
		}
	}
}