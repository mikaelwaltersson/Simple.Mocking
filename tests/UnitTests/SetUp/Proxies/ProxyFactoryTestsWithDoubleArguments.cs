using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithDoubleArguments : ProxyFactoryTests<double>
	{
		protected override void OnSetUp()
		{
			expectedInput = 0.123456f;
			expectedInputIndex = 3;
			expectedOutput = 0.654321f;
		}
	}
}