using System;

using NUnit.Framework;

using Simple.Mocking.SetUp.Actions;

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