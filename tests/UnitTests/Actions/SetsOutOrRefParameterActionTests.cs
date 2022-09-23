using NUnit.Framework;

using Simple.Mocking.SetUp.Actions;

namespace Simple.Mocking.UnitTests.Actions
{
    [TestFixture]
	public class SetsOutOrRefParameterActionTests : ActionTestsBase
	{
		[Test]
		public void ExecuteFor()
		{
			var value = new object();
			var invocation = CreateInvocation();

			new SetsOutOrRefParameterAction(0, value).ExecuteFor(invocation);

			Assert.AreSame(value, invocation.ParameterValues[0]);
		}
	}
}