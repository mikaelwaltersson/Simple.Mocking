using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithUnsignedShortArguments : ProxyFactoryTests<ushort>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0xF123;
			expectedInputIndex = 11;
			expectedOutput = 0xF321;
		}
	}
}