using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Actions;
using Simple.Mocking.SetUp.Proxies;


namespace Simple.Mocking.UnitTests.SetUp
{
	[TestFixture]
	public class ExpectationTests  
	{
		InvocationMatcher invocationMatcher = new InvocationMatcher(null, typeof(object).GetMethod("ToString"), new object[0]);
		NumberOfInvocationsConstraint exactlyOnceNumberOfInvocationsConstraint = new NumberOfInvocationsConstraint(1, 1);
		NumberOfInvocationsConstraint anyNumberOfInvocationsConstraint = new NumberOfInvocationsConstraint(null, null);
		NumberOfInvocationsConstraint neverNumberOfInvocationsConstraint = new NumberOfInvocationsConstraint(0, 0);

		[Test]
		public void CantCreateExpectationWithNullArguments()
		{
			try
			{
				new Expectation(null, exactlyOnceNumberOfInvocationsConstraint);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{				
			}

			try
			{
				new Expectation(invocationMatcher, null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
			
		}

		[Test]
		public void CantPassNullArgumentsToAddAction()
		{
			var expectation = new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint);

			try
			{
				expectation.AddAction(null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void TryMeet()
		{
			var toStringInvocation = new Invocation(null, typeof(object).GetMethod("ToString"), null, new object[0], null);
			var getHashCodeInvocation = new Invocation(null, typeof(object).GetMethod("GetHashCode"), null, new object[0], null);

			var expectToStringOnce = new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint);
			var expectToStringNever = new Expectation(invocationMatcher, neverNumberOfInvocationsConstraint);

			Assert.IsFalse(expectToStringOnce.TryMeet(getHashCodeInvocation));

			Assert.IsTrue(expectToStringOnce.TryMeet(toStringInvocation));
			Assert.IsFalse(expectToStringOnce.TryMeet(toStringInvocation));

			Assert.IsFalse(expectToStringNever.TryMeet(toStringInvocation));
		}

		[Test]
		public void TryMeetExecutesActions()
		{
			var toStringInvocation = new Invocation(null, typeof(object).GetMethod("ToString"), null, new object[0], null);
			
			var expectToStringOnce = new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint);

			var action1 = new Action();
			var action2 = new Action();

			expectToStringOnce.AddAction(action1);
			expectToStringOnce.AddAction(action2);

			expectToStringOnce.TryMeet(toStringInvocation);


			Assert.AreEqual(1, action1.ExecuteCount);
			Assert.AreEqual(1, action2.ExecuteCount);
		}

		[Test]
		public void HasBeenMeetIsTrueWhenNumberOfInvocationMatches()
		{
			Assert.IsFalse(new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint).HasBeenMet);
			Assert.IsTrue(new Expectation(invocationMatcher, anyNumberOfInvocationsConstraint).HasBeenMet);
		}

		[Test]
		public void ToStringCanBeInvokedForAnyInvocationMatcher()
		{
			new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint).ToString();
		}

		class Action : IAction
		{
			public int ExecuteCount;

			public void ExecuteFor(IInvocation invocation)
			{
				ExecuteCount++;
			}
		}
	}

	[TestFixture]
	public class MockNameTests
	{
		[Test]
		public void GetUniqueName()
		{
			var scope = new Scope();

			Assert.AreEqual("object", MockName<object>.GetUniqueInScope(scope));
			Assert.AreEqual("object2", MockName<object>.GetUniqueInScope(scope));
			Assert.AreEqual("object3", MockName<object>.GetUniqueInScope(scope));

			Assert.AreEqual("string", MockName<string>.GetUniqueInScope(scope));
			Assert.AreEqual("string2", MockName<string>.GetUniqueInScope(scope));
			Assert.AreEqual("string3", MockName<string>.GetUniqueInScope(scope));

			Assert.AreEqual("iMyDelegate", MockName<IMyDelegate>.GetUniqueInScope(scope));
			Assert.AreEqual("imyDelegate", MockName<IMYDelegate>.GetUniqueInScope(scope));
			Assert.AreEqual("myInterface", MockName<IMyInterface>.GetUniqueInScope(scope));
		}

		delegate void IMYDelegate();

		delegate void IMyDelegate();

		interface IMyInterface
		{
		}

		class Scope : IMockNameScope
		{
			HashSet<string> names = new HashSet<string>();

			public bool Register(string name)
			{
				return names.Add(name);
			}
		}
	}
}
