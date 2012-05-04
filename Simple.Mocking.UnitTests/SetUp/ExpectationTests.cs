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
	    Action ignoredAction;


		[Test]
		public void CantCreateExpectationWithNullArguments()
		{
		    Assert.Throws<ArgumentNullException>(() => new Expectation(null, exactlyOnceNumberOfInvocationsConstraint));
            Assert.Throws<ArgumentNullException>(() => new Expectation(invocationMatcher, null));	
		}

		[Test]
		public void CantPassNullArgumentsToAddAction()
		{
			var expectation = new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint);

            Assert.Throws<ArgumentNullException>(() => expectation.AddAction(null));	
		}

		[Test]
		public void TryMeet()
		{
			var toStringInvocation = new Invocation(null, typeof(object).GetMethod("ToString"), null, new object[0], null, 0);
			var getHashCodeInvocation = new Invocation(null, typeof(object).GetMethod("GetHashCode"), null, new object[0], null, 0);

			var expectToStringOnce = new Expectation(invocationMatcher, exactlyOnceNumberOfInvocationsConstraint);
			var expectToStringNever = new Expectation(invocationMatcher, neverNumberOfInvocationsConstraint);

            Assert.IsFalse(expectToStringOnce.TryMeet(getHashCodeInvocation, out ignoredAction));

            Assert.IsTrue(expectToStringOnce.TryMeet(toStringInvocation, out ignoredAction));
            Assert.IsFalse(expectToStringOnce.TryMeet(toStringInvocation, out ignoredAction));

            Assert.IsFalse(expectToStringNever.TryMeet(toStringInvocation, out ignoredAction));
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
	}
}
