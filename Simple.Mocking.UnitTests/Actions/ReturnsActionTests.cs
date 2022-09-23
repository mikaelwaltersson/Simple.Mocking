using NUnit.Framework;

using Simple.Mocking.SetUp.Actions;

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