﻿using NUnit.Framework;

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

            Assert.Throws<ExpectationsException>(() => myObject.MyEmptyMethod());
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
