using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithIntArguments : ProxyFactoryTests<int>
	{
		protected override void OnSetUp()
		{
			expectedInput = -123;
			expectedInputIndex = 6;
			expectedOutput = -321;
		}
	}
}