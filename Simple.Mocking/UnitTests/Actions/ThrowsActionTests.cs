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
	public class ThrowsActionTests : ActionTestsBase
	{
		[Test]
		public void ExecuteFor()
		{
			var exception = new Exception();
			var invocation = CreateInvocation();

			try
			{
				new ThrowsAction(exception).ExecuteFor(invocation);
			}
			catch (Exception ex)
			{
				Assert.AreSame(exception, ex);
			}
		}
	}
}