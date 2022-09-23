using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithClassTypeArguments : ProxyFactoryTests<ProxyFactoryTestsWithClassTypeArguments.MyClassType>
	{
		protected override void OnSetUp()
		{
			expectedInput = new MyClassType();
			expectedInputIndex = new MyClassType();
			expectedOutput = new MyClassType();
		}

		public class MyClassType
		{
		}

	}
}