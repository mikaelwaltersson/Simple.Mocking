using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp.Actions;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.Actions
{
	[TestFixture]
	public class ReturnsActionTests : ActionTestsBase
	{
		[Test]
		public void ExecuteFor()
		{
			var value = new object();
			var invocation = CreateInvocation();
			
			new ReturnsAction(value).ExecuteFor(invocation);

			Assert.AreSame(value, invocation.ReturnValue);
		}
	}
}