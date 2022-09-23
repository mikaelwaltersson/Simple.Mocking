using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithLongArguments : ProxyFactoryTests<long>
	{
		protected override void OnSetUp()
		{
			expectedInput = -1234;
			expectedInputIndex = 7;
			expectedOutput = -4321;
		}
	}
}