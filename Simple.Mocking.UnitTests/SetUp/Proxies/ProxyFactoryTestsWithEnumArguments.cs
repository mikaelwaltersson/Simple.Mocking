using NUnit.Framework;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class ProxyFactoryTestsWithEnumArguments : ProxyFactoryTests<ProxyFactoryTestsWithEnumArguments.MyEnum>
	{
		protected override void OnSetUp()
		{
			expectedInput = MyEnum.A;
			expectedInputIndex = MyEnum.B;
			expectedOutput = MyEnum.C;
		}

		public enum MyEnum
		{
			A, B, C
		}
	}
}