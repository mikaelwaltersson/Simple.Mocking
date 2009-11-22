using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
	[TestFixture]
	public class AssertExpectationsTests
	{
		[Test]
		public void ForSingleMock()
		{
			var myObject = Mock.Interface<IMyObject>();

			Expect.Once.MethodCall(() => myObject.MyMethod(1));
			Expect.Once.MethodCall(() => myObject.MyMethod(2));

			myObject.MyMethod(1);
			myObject.MyMethod(2);

			AssertExpectations.IsMetFor(myObject);
		}

		[Test]
		public void ForScope()
		{
			var expectationScope = new ExpectationScope();

			var myObject1 = Mock.Interface<IMyObject>(expectationScope);
			var myObject2 = Mock.Interface<IMyObject>(expectationScope);

			Expect.Once.MethodCall(() => myObject1.MyMethod(1));
			Expect.Once.MethodCall(() => myObject2.MyMethod(2));

			myObject1.MyMethod(1);
			myObject2.MyMethod(2);

			AssertExpectations.IsMetFor(expectationScope);
		}

		[Test]
		public void NotMet()
		{
			var myObject = Mock.Interface<IMyObject>();

			Expect.Once.MethodCall(() => myObject.MyMethod(1));

			try
			{
				AssertExpectations.IsMetFor(myObject);
				Assert.Fail();
			}
			catch (ExpectationsException ex)
			{
				Assert.IsTrue(ex.Message.StartsWith("All expectations has not been met, expected:"));
			}
		}
	}
}