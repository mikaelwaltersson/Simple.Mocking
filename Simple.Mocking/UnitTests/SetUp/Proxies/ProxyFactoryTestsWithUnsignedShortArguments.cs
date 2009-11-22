using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithUnsignedShortArguments : ProxyFactoryTests<ushort>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0xF123;
			expectedInputIndex = 11;
			expectedOutput = 0xF321;
		}
	}
}