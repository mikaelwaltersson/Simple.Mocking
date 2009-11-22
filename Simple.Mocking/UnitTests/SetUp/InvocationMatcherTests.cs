using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;
using Simple.Mocking.Syntax;


namespace Simple.Mocking.UnitTests.SetUp
{
	[TestFixture]
	public class InvocationMatcherTests
	{
		IMyObject myObject = new MyObject();
		MyDelegate myDelegate = new MyObject().MethodWithReturnValue;

		[Test]
		public void ForMethodCall()
		{
			Expression<Action> expression = () => myObject.Method(42);

			var invocationMatcher = InvocationMatcher.ForMethodCall(expression);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetMethod("Method"), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { 42 }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForMethodCallWithInvalidUsageOfAnyValueAsRefOrOut()
		{
			Expression<Action> expression = () => myObject.Method(Any<int>.Value.AsRefOrOut);

			try
			{
				InvocationMatcher.ForMethodCall(expression);
			}
			catch (ArgumentException)
			{				
			}
		}

		[Test]
		public void ForMethodCallWithParameterValueConstraint()
		{
			Expression<Func<int>> expression = () => myObject.MethodWithReturnValue(Any<int>.Value.Matching(i => i % 2 == 0));

			var invocationMatcher = InvocationMatcher.ForMethodCall(expression);

			var parameterValueConstraint = invocationMatcher.ParameterValueConstraints[0];

			Assert.IsInstanceOf<MatchingPredicateValueConstraint<int>>(parameterValueConstraint);
			Assert.AreEqual(Any<int>.Value.Matching(i => i%2 == 0).ToString(), parameterValueConstraint.ToString());
		}

		[Test]
		public void ForMethodCallWithDelegate()
		{
			Expression<Action> expression = () => myDelegate(42);

			var invocationMatcher = InvocationMatcher.ForMethodCall(expression);

			Assert.IsInstanceOf(typeof(InvocationMatcher.DelegateInvocationMatcher), invocationMatcher);
			Assert.AreSame(myDelegate, invocationMatcher.Target);
			Assert.AreEqual(myDelegate.Method, invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { 42 }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForMethodCallWithInvalidExpression()
		{
			var invalidExpressions =
				new Expression<Func<object>>[]
				{
					() => myObject.MethodWithReturnValue(42) + 1,
					() => myObject.Property,
					() => myObject[0]
				};

			foreach (var expression in invalidExpressions)
			{
				try
				{
					InvocationMatcher.ForMethodCall(expression);
					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}

		[Test]
		public void ForGenericMethodCall()
		{
			Expression<Action> expression = () => myObject.GenericMethod("1", Any<string>.Value);

			var invocationMatcher = InvocationMatcher.ForMethodCall(expression);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetMethod("GenericMethod").MakeGenericMethod(new[] { typeof(string), typeof(string) }), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { "1", Any<string>.Value }, invocationMatcher.ParameterValueConstraints);
		}



		[Test]
		public void ForPropertyGet()
		{
			Expression<Func<int>> expression = () => myObject.Property;

			var invocationMatcher = InvocationMatcher.ForPropertyGet(expression);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetProperty("Property").GetGetMethod(), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[0], invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForPropertyGetWithIndex()
		{
			Expression<Func<string>> expression = () => myObject[Any<int>.Value];

			var invocationMatcher = InvocationMatcher.ForPropertyGet(expression);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetProperty("Item").GetGetMethod(), invocationMatcher.Method);

			var parameterValueConstraint = invocationMatcher.ParameterValueConstraints[0];

			Assert.IsInstanceOf<AnyValueConstraint<int>>(parameterValueConstraint);
		}

		[Test]
		public void ForPropertyGetWithInvalidExpression()
		{
			var invalidExpressions =
				new Expression<Func<object>>[]
				{
					() => myObject.MethodWithReturnValue(42),
					() => myObject.Property + 1,
					() => myObject[1].ToLower(),
				};

			foreach (var expression in invalidExpressions)
			{
				try
				{
					InvocationMatcher.ForPropertyGet(expression);
					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}


		[Test]
		public void ForPropertySet()
		{
			Expression<Func<int>> expression = () => myObject.Property;

			var invocationMatcher = InvocationMatcher.ForPropertySet(expression, 42);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetProperty("Property").GetSetMethod(), invocationMatcher.Method);
			CollectionAssert.AreEqual(new []{ 42 }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForPropertySetWithIndex()
		{
			Expression<Func<string>> expression = () => myObject[Any<int>.Value];

			var invocationMatcher = InvocationMatcher.ForPropertySet(expression, Any<string>.Value);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetProperty("Item").GetSetMethod(), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { Any<int>.Value, Any<string>.Value }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForEventAdd()
		{			
			var invocationMatcher = InvocationMatcher.ForEventAdd(myObject, "Event", Any<EventHandler>.Value);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetEvent("Event").GetAddMethod(), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { Any<EventHandler>.Value }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForEventRemove()
		{
			var invocationMatcher = InvocationMatcher.ForEventRemove(myObject, "Event", Any<EventHandler>.Value);

			Assert.AreSame(myObject, invocationMatcher.Target);
			Assert.AreEqual(typeof(IMyObject).GetEvent("Event").GetRemoveMethod(), invocationMatcher.Method);
			CollectionAssert.AreEqual(new object[] { Any<EventHandler>.Value }, invocationMatcher.ParameterValueConstraints);
		}

		[Test]
		public void ForEventAddOnNonExistingEvent()
		{
			try
			{
				InvocationMatcher.ForEventAdd(myObject, "Event2", Any<EventHandler>.Value);
				Assert.Fail();
			}
			catch (ArgumentException)
			{				
			}
		}

		[Test]
		public void ForAnyInvocationOnTarget()
		{
			var invocationMatcher = InvocationMatcher.ForAnyInvocationOn(myObject);

			Assert.IsInstanceOf(typeof(InvocationMatcher.AnyInvocationOnTargetMatcher<IMyObject>), invocationMatcher);
			Assert.IsNull(invocationMatcher.Method);
			Assert.IsNull(invocationMatcher.ParameterValueConstraints);
			Assert.AreEqual(myObject + ".*", invocationMatcher.ToString());
		}

		[Test]
		public void Matches()
		{
			var invocationMatcher = new InvocationMatcher(myObject, typeof(IMyObject).GetMethod("Method"), new object[] { 42 });

			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", 42)));
			
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", 43)));
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", 42, 0)));
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(new MyObject(), "Method", 42)));
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(myObject, "MethodWithReturnValue", 42)));
		}

		[Test]
		public void MatchesWithParameterValueConstraints()
		{
			var invocationMatcher = new InvocationMatcher(myObject, typeof(IMyObject).GetMethod("Method"), new object[] { Any<int>.Value });

			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", int.MinValue)));
			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", 42)));
			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", int.MaxValue)));

			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", "42")));
		}

		[Test]
		public void MatchesWithNullValues()
		{
			var invocationMatcher = new InvocationMatcher(myObject, typeof(IMyObject).GetMethod("Method"), new object[] { null });

			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", (object)null)));
			
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(myObject, "Method", 42)));		
		}

		[Test]
		public void MatchesGenericMethod()
		{
			var invocationMatcher = 
				new InvocationMatcher(
					myObject, 
					typeof(IMyObject).GetMethod("GenericMethod").MakeGenericMethod(new[] { typeof(string), typeof(int) }), 
					new object[] { "1", 1 });

			Assert.IsTrue(invocationMatcher.Matches(CreateGenericMethodInvocation(myObject, "GenericMethod", new[] { typeof(string), typeof(int) },  "1", 1)));
			
			Assert.IsFalse(invocationMatcher.Matches(CreateGenericMethodInvocation(myObject, "GenericMethod", new[] { typeof(int), typeof(string)  }, 1, "1")));
			Assert.IsFalse(invocationMatcher.Matches(CreateGenericMethodInvocation(myObject, "GenericMethod", new[] { typeof(string), typeof(int) }, "2", 2)));
		}

		[Test]
		public void MatchesDelegateMethod()
		{
			var invocationMatcher = new InvocationMatcher.DelegateInvocationMatcher(myDelegate, new[] { Any<int>.Value });
			var otherDelegate = (MyDelegate)new MyObject().MethodWithReturnValue;

			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(myDelegate.Target, myDelegate.Method.Name, 42)));
			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(otherDelegate.Target, otherDelegate.Method.Name, 42)));
		}

		[Test]
		public void MatchesAnyInvocationOnTarget()
		{
			var target1 = new MyObject();
			var target2 = new MyObject();

			var invocationMatcher = new InvocationMatcher.AnyInvocationOnTargetMatcher<IMyObject>(target1);

			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(target1, "Method")));
			Assert.IsTrue(invocationMatcher.Matches(CreateMethodInvocation(target1, "MethodWithReturnValue")));

			Assert.IsFalse(invocationMatcher.Matches(CreateMethodInvocation(target2, "MethodWithReturnValue")));
		}
		

		[Test]
		public void ToStringCanBeInvokedForAnyInvocationMatcher()
		{
			new InvocationMatcher(null, null, null).ToString();
		}

		IInvocation CreateMethodInvocation(object target, string methodName, params object[] parameterValues)
		{
			return new Invocation(target as IProxy, typeof(IMyObject).GetMethod(methodName), null, parameterValues, null);
		}

		IInvocation CreateGenericMethodInvocation(object target, string methodName, Type[] genericArguments, params object[] parameterValues)
		{
			return new Invocation(target as IProxy, typeof(IMyObject).GetMethod(methodName), genericArguments, parameterValues, null);
		}


		delegate int MyDelegate(int value);

		interface IMyObject
		{
			void Method(object value);
			int MethodWithReturnValue(int value);
			void GenericMethod<A, B>(A a, B b);
			string this[int i] { get; set; }
			int Property { get; set; }
			event EventHandler Event;
		}

		class MyObject : IMyObject, IProxy
		{
			public void Method(object value)
			{
			}

			public int MethodWithReturnValue(int value)
			{
				return value;
			}

			public void GenericMethod<A, B>(A a, B b)
			{				
			}


			public string this[int i] { get { return string.Empty; } set { } }
			public int Property { get; set; }
			public event EventHandler Event;

			Type IProxy.ProxiedType
			{
				get { throw new NotSupportedException(); }
			}

			object IProxy.BaseObject
			{
				get { return this; }
			}

			IInvocationInterceptor IProxy.InvocationInterceptor
			{
				get { throw new NotSupportedException(); }
			}
		}

	}
}