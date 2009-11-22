using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithByteArguments : ProxyFactoryTests<byte>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0xF1;
			expectedInputIndex = 1;
			expectedOutput = 0xF2;
		}
	}
}