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

		[Test]
		public void ForStub()
		{
			var myObject = Mock.Interface<IMyObject>();

			Expect.PropertyGet(() => myObject.MyProperty).Returns(41);
			Expect.AnyInvocationOn(myObject);
			

			myObject.MyMethod(1);
			myObject.MyMethod(2);
			myObject.MyMethod(3);
			
			myObject.MyGenericMethod("hello world");
			myObject.MyGenericMethod("Hello World!!!");
			myObject.MyGenericMethod(4311077043);
			
			myObject.MyProperty = (int)myObject.MyProperty + 1;

			var eventHandler = (EventHandler)delegate { };

			myObject.MyEvent -= eventHandler;
			myObject.MyEvent += eventHandler;

			AssertExpectations.
				IsMetForCallTo.
				MethodCall(() => myObject.MyMethod(Any<int>.Value)).
				MethodCall(() => myObject.MyGenericMethod(Any<string>.Value.Matching(s => s.StartsWith("Hel")))).
				PropertyGet(() => myObject.MyProperty).
				PropertySet(() => myObject.MyProperty, 42).
				EventAdd(myObject, "MyEvent", Any<EventHandler>.Value).
				EventRemove(myObject, "MyEvent", Any<EventHandler>.Value);

			try
			{
				AssertExpectations.IsMetForCallTo.MethodCall(() => myObject.MyMethod(4));
				Assert.Fail();
			}
			catch (ExpectationsException ex)
			{
				Assert.IsTrue(ex.Message.StartsWith("Expected invocation 'myObject.MyMethod(4)' was not made, invocations:"));
			}
		}

		[Test]
		public void NumberOfCallsExceededButExceptionCatchedByUserCode()
		{
			var myObject = Mock.Interface<IMyObject>();

			Expect.Once.MethodCall(() => myObject.MyMethod(1));

			try
			{
				myObject.MyMethod(1);
				myObject.MyMethod(1);
			}
			catch (ExpectationsException)
			{				
			}

			try
			{
				AssertExpectations.IsMetFor(myObject);
				Assert.Fail();
			}
			catch (ExpectationsException)
			{
			}
		}
	}
}