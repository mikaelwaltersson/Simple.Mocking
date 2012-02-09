using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
	[TestFixture]
	public class MockTests
	{
		[Test]
		public void InvokingUnexpectedActionThrowsException()
		{
			var myObject = Mock.Interface<IMyObject>();

			try
			{
				myObject.MyEmptyMethod();

				Assert.Fail();
			}
			catch (ExpectationsException)
			{
			}
		}

		[Test]
		public void EqualsComparesBaseObjectNotProxyInstance()
		{
			var myObject1 = Mock.Interface<IMyObject>();
			var myObject2 = Mock.Interface<IMyObject>();

			Assert.IsTrue(myObject1.Equals(myObject1));
			Assert.IsFalse(myObject1.Equals(myObject2));
		}
	}
}
