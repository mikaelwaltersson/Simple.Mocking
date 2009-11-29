using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Delegates;
using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
	[TestFixture]
	public class ExpectTests
	{
		IMyObject myObject;


		[SetUp]
		public void Initialize()
		{
			myObject = Mock.Interface<IMyObject>();
		}


		[Test]
		public void ExpectMethodCalled()
		{
			Expect.MethodCall(() => myObject.MyMethod(Any<int>.Value));

			myObject.MyMethod(1);
			myObject.MyMethod(2);
			myObject.MyMethod(3);
		}

		[Test]
		public void ExpectMethodCalledOnce()
		{
			Expect.Once.MethodCall(() => myObject.MyMethod(Any<int>.Value));

			myObject.MyMethod(1);
		}

		[Test]
		public void ExpectMethodWithReturnValueCalled()
		{
			Expect.
				MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).
				Returns(42);

			int returnValue = myObject.MyMethodWithReturnValue(123);

			Assert.AreEqual(42, returnValue);
		}

		[Test]
		public void ExpectMethodWithOutParameterCalled()
		{
			Expect.
				MethodCall(() => myObject.MyMethodWithOutParameter(Any<int>.Value, out Any<int>.Value.AsRefOrOut)).
				SetsOutOrRefParameter(1, 42);

      
			int outValue;

			myObject.MyMethodWithOutParameter(123, out outValue);

			Assert.AreEqual(42, outValue);
		}

		[Test]
		public void ExpectMethodWithRefParameterCalled()
		{
			Expect.
				MethodCall(() => myObject.MyMethodWithRefParameter(ref Any<int>.Value.AsRefOrOut)).
				SetsOutOrRefParameter(0, 42);


			int value = 123;

			myObject.MyMethodWithRefParameter(ref value);

			Assert.AreEqual(42, value);
		}

		[Test]
		public void ExpectGenericMethodCalled()
		{
			Expect.MethodCall(() => myObject.MyGenericMethod(Any<string>.Value));
			Expect.MethodCall(() => myObject.MyGenericMethod(Any<long>.Value));

			myObject.MyGenericMethod(string.Empty);
			myObject.MyGenericMethod(0L);
		}

		[Test]
		public void ExpectDelegateMethodCalled()
		{
			var myDelegate = Mock.Delegate<MyDelegate>();

			Expect.MethodCall(() => myDelegate(Any<int>.Value));

			myDelegate(1);
		}

		[Test]
		public void ExpectDelegateMethodWithReturnValueCalled()
		{
			var myDelegateWithReturnValue = Mock.Delegate<MyDelegateWithReturnValue>();

			Expect.
				MethodCall(() => myDelegateWithReturnValue(Any<int>.Value)).
				Returns(42);


			int returnValue = myDelegateWithReturnValue(123);

			Assert.AreEqual(42, returnValue);
		}

		[Test]
		public void ExpectDelegateMethodWithOutParameterCalled()
		{
			var myDelegateWithOutParameter = Mock.Delegate<MyDelegateWithOutParameter>();

			Expect.
				MethodCall(() => myDelegateWithOutParameter(Any<int>.Value, out Any<int>.Value.AsRefOrOut)).
				SetsOutOrRefParameter(1, 42);


			int outValue;

			myDelegateWithOutParameter(123, out outValue);

			Assert.AreEqual(42, outValue);
		}

		[Test]
		public void ExpectDelegateMethodWithRefParameterCalled()
		{
			var myDelegateWithRefParameter = Mock.Delegate<MyDelegateWithRefParameter>();

			Expect.
				MethodCall(() => myDelegateWithRefParameter(ref Any<int>.Value.AsRefOrOut)).
				SetsOutOrRefParameter(0, 42);


			int value = 123;

			myDelegateWithRefParameter(ref value);

			Assert.AreEqual(42, value);
		}


		[Test]
		public void ExpectPropertyGetterCalled()
		{
			Expect.PropertyGet(() => myObject.MyProperty).Returns(123);

			Assert.AreEqual(123, myObject.MyProperty);
			Assert.AreEqual(123, myObject.MyProperty);
			Assert.AreEqual(123, myObject.MyProperty);
		}

		[Test]
		public void ExpectPropertyGetterCalledOnce()
		{
			Expect.Once.PropertyGet(() => myObject.MyProperty).Returns(123);

			Assert.AreEqual(123, myObject.MyProperty);
		}

		[Test]
		public void ExpectPropertySetterCalled()
		{
			Expect.PropertySet(() => myObject.MyProperty, Any<int>.Value);

			myObject.MyProperty = 1;
			myObject.MyProperty = 2;
			myObject.MyProperty = 3;
		}

		[Test]
		public void ExpectPropertySetterCalledOnce()
		{
			Expect.Once.PropertySet(() => myObject.MyProperty, Any<int>.Value);

			myObject.MyProperty = 1;
		}

		[Test]
		public void ExpectIndexedPropertyGetterCalled()
		{
			Expect.
				PropertyGet(() => myObject[Any<int>.Value]).
				Returns(10203);

			Assert.AreEqual(10203, myObject[0]);
		}

		[Test]
		public void ExpectIndexedPropertySetterCalled()
		{
			Expect.PropertySet(() => myObject[Any<int>.Value], Any<int>.Value);

			myObject[0] = 10203;
		}

		[Test]
		public void ExpectEventAdderCalled()
		{
			Expect.EventAdd(myObject, "MyEvent", Any<EventHandler>.Value);

			myObject.MyEvent += delegate { };
			myObject.MyEvent += delegate { };
			myObject.MyEvent += delegate { };
		}

		[Test]
		public void ExpectEventAdderCalledOnce()
		{
			Expect.Once.EventAdd(myObject, "MyEvent", Any<EventHandler>.Value);

			myObject.MyEvent += delegate { };
		}

		[Test]
		public void ExpectEventRemoverCalled()
		{
			Expect.EventRemove(myObject, "MyEvent", Any<EventHandler>.Value);

			myObject.MyEvent -= delegate { };
		}

		[Test]
		public void ExpectInvokedOnce()
		{
			Expect.Once.MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedAtLeastOnce()
		{
			Expect.AtLeastOnce.MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedAtMostOnce()
		{
			Expect.AtMostOnce.MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedAtLeastNTimes()
		{
			Expect.AtLeast(3).MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedAtMostNTimes()
		{
			Expect.AtMost(3).MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedExactlyNTimes()
		{
			Expect.Exactly(2).MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvokedBetweenXAndYTimes()
		{
			Expect.Between(2, 4).MethodCall(() => myObject.MyEmptyMethod());

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
		}

		[Test]
		public void ExpectInvocationReturns()
		{
			Expect.MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).Returns(1);

			Assert.AreEqual(1, myObject.MyMethodWithReturnValue(2));
			Assert.AreEqual(1, myObject.MyMethodWithReturnValue(3));
			Assert.AreEqual(1, myObject.MyMethodWithReturnValue(4));
		}

		[Test]
		public void ExpectInvocationExecutes()
		{
			int invocationCount = 0;

			Expect.
				MethodCall(() => myObject.MyEmptyMethod()).
				Executes(() => invocationCount++);

			Expect.
				MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).
				Executes(() => invocationCount);

			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();
			myObject.MyEmptyMethod();

			Assert.AreEqual(3, myObject.MyMethodWithReturnValue(0));
		}

		[Test]
		public void ExpectInvocationExecutesWithParameters()
		{
			int invocationCount = 0;

			Expect.
				MethodCall(() => myObject.MyMethod(Any<int>.Value)).
				Executes(parameters => invocationCount += (int)parameters[0]);
			
			Expect.
				MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).
				Executes(parameters => invocationCount);

			myObject.MyMethod(1);
			myObject.MyMethod(2);
			myObject.MyMethod(3);			

			Assert.AreEqual(6, myObject.MyMethodWithReturnValue(0));
		}

		[Test]
		public void ExpectInvocationThrows()
		{
			var exceptionThrown1 = new Exception();
			var exceptionThrown2 = new Exception();

			Expect.MethodCall(() => myObject.MyEmptyMethod()).Throws(exceptionThrown1);
			Expect.MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).Throws(exceptionThrown2);

			try
			{
				myObject.MyEmptyMethod();
			}
			catch (Exception ex)
			{
				Assert.AreSame(exceptionThrown1, ex);
			}

			try
			{
				myObject.MyMethodWithReturnValue(0);
			}
			catch (Exception ex)
			{
				Assert.AreSame(exceptionThrown2, ex);
			}
		}

		[Test]
		public void ExpectInvocationSetsRefOrOutParameter()
		{
			Expect.
				MethodCall(() => myObject.MyMethodWithOutParameter(Any<int>.Value, out Any<int>.Value.AsRefOrOut)).
				SetsOutOrRefParameter(1, 101);

			int outValue;
			myObject.MyMethodWithOutParameter(0, out outValue);

			Assert.AreEqual(101, outValue);
		}

		[Test]
		public void ExpectAnyInvocationOn()
		{		
			Expect.AnyInvocationOn(myObject);

			myObject.MyMethod(1);
			myObject.MyMethod(2);
			myObject.MyMethod(3);

			myObject.MyProperty = 1;
			myObject.MyEvent += delegate { };

			int outValue;
			myObject.MyMethodWithOutParameter(0, out outValue);

			myObject.MyGenericMethod(1.0);
		}
	}
}
