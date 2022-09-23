using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithCharArguments : ProxyFactoryTests<char>
	{
		protected override void OnSetUp()
		{
			expectedInput = 'a';
			expectedInputIndex = 'b';
			expectedOutput = 'c';
		}
	}
}