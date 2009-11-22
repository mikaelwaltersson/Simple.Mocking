using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTestsWithCharArguments : ProxyFactoryTests<char>
	{
		protected override void OnSetUp()
		{
			expectedInput = 'a';
			expectedInputIndex = 'b';
			expectedOutput = 'c';
		}
	}
}