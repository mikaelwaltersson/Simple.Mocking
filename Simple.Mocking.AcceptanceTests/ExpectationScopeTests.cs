using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
	[TestFixture]
	public class ExpectationScopeTests
	{
		[Test]
		public void MeetExpectationsInScope()
		{
			var expectationScope = new ExpectationScope();

			var myObject1 = Mock.Interface<IMyObject>(expectationScope);
			var myObject2 = Mock.Interface<IMyObject>(expectationScope);
			
			Expect.Once.MethodCall(() => myObject1.MyMethod(1));			
			Expect.Once.MethodCall(() => myObject2.MyMethod(2));

			using (expectationScope.BeginOrdered())
			{
				Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(3));
                Expect.Between(0, 3).MethodCall(() => myObject1.MyGenericMethod("WILL PROBABLY NEVER BE INVOKED"));	
				Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(Any<int>.Value.Matching(i => i > 10)));

				using (expectationScope.BeginUnordered())
				{
					Expect.AtLeastOnce.MethodCall(() => myObject2.MyMethod(4));
					Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(5));
				}
			}

			myObject1.MyMethod(1);
			myObject2.MyMethod(2);
			
			myObject1.MyMethod(3);
			myObject1.MyMethod(3);

			myObject1.MyMethod(13);
			myObject1.MyMethod(12);
			myObject1.MyMethod(11);

			myObject1.MyMethod(5);
			myObject2.MyMethod(4);		

			AssertExpectations.IsMetFor(expectationScope);
		}

		[Test]
		public void FailureToMeetExpectationsInScopeThrowsDescribingException()
		{
			var expectationScope = new ExpectationScope();

			var myObject1 = Mock.Interface<IMyObject>(expectationScope);
			var myObject2 = Mock.Interface<IMyObject>(expectationScope);
			var myObject3 = Mock.Interface<IMyObject>(expectationScope);

			Expect.Once.MethodCall(() => myObject1.MyMethod(1));
			Expect.Once.MethodCall(() => myObject2.MyMethod(2));

			using (expectationScope.BeginOrdered())
			{
				Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(3));
				Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(Any<int>.Value.Matching(i => i > 10)));

				using (expectationScope.BeginUnordered())
				{
					Expect.AtLeastOnce.MethodCall(() => myObject2.MyMethod(4));
					Expect.AtLeastOnce.MethodCall(() => myObject1.MyMethod(5));
				}
			}

			Expect.AnyInvocationOn(myObject3);


			myObject1.MyMethod(1);
			myObject2.MyMethod(2);

			myObject1.MyMethod(3);
			myObject1.MyMethod(3);
			myObject1.MyMethod(3);

			try
			{
				myObject1.MyMethod(4);	
			}
			catch (ExpectationsException ex)
			{
				Assert.AreEqual(
					"Unexpected invocation 'myObject.MyMethod(4)', expected:\r\n" +
					"\r\n" +
					"(invoked: 1 of 1) myObject.MyMethod(1)\r\n" +
					"(invoked: 1 of 1) myObject2.MyMethod(2)\r\n" +
					"In order {\r\n" +
					"  (invoked: 3 of 1..*) myObject.MyMethod(3)\r\n" +
					"  (invoked: 0 of 1..*) myObject.MyMethod(Any<Int32>.Value.Matching(i => (i > 10)))\r\n" +
					"  Unordered {\r\n" +
					"    (invoked: 0 of 1..*) myObject2.MyMethod(4)\r\n" +
					"    (invoked: 0 of 1..*) myObject.MyMethod(5)\r\n" +
					"  }\r\n" +
					"}\r\n" +
					"(invoked: 0 of *) myObject3.*\r\n" +
                    "\r\n" +
                    "Unexpected invocations:\r\n" +
                    "  myObject.MyMethod(4)\r\n"+
                    "\r\n",
					ex.Message);
			}
		}

	}
}