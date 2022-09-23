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