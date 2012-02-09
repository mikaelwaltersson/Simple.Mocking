using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithBoolArguments : ProxyFactoryTests<bool>
	{
		protected override void OnSetUp()
		{
			expectedInput = true;
			expectedInputIndex = false;
			expectedOutput = false;
		}
	}
}