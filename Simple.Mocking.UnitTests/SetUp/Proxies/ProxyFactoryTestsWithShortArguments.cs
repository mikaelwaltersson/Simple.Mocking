using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithShortArguments : ProxyFactoryTests<short>
	{
		protected override void OnSetUp()
		{
			expectedInput = -12;
			expectedInputIndex = 8;
			expectedOutput = -21;
		}
	}
}