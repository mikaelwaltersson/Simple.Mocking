using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithUnsignedIntArguments : ProxyFactoryTests<uint>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0xF0000123;
			expectedInputIndex = 6;
			expectedOutput = 0xF0000321;
		}
	}
}