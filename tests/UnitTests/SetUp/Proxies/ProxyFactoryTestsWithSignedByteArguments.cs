using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithSignedByteArguments : ProxyFactoryTests<sbyte>
	{
		protected override void OnSetUp()
		{
			expectedInput = -128;
			expectedInputIndex = 9;
			expectedOutput = 127;
		}
	}
}