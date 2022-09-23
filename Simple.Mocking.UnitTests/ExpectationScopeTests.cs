using System;
using System.Reflection;
using System.Collections.Generic;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests
{
    [TestFixture]
	public class ExpectationScopeTests
	{
		ExpectationScope expectationScope;
		Invocation invocation1;
		Invocation invocation2;
		Invocation invocation3;


		[SetUp]
		public void SetUp()
		{
			expectationScope = new ExpectationScope();

			invocation1 = new Invocation();
			invocation2 = new Invocation();
			invocation3 = new Invocation();
		}

		[Test]
		public void MeetUnorderedChildScope()
		{
			using (expectationScope.BeginUnordered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation3));
			}
	
			TryMeetSucceeds(invocation2);

			TryMeetSucceeds(invocation3);

			TryMeetSucceeds(invocation1);

			AssertHasBeenMet();
		}

		[Test]
		public void FailToMeetUnorderedChildScopeMissingInvocations()
		{
			using (expectationScope.BeginUnordered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation3));
			}


			TryMeetSucceeds(invocation1);

			TryMeetSucceeds(invocation2);

			AssertHasNotBeenMet();
		}
		
		[Test]
		public void FailToMeetUnorderedChildScopeWrongInvocation()
		{
			using (expectationScope.BeginUnordered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
			}

			TryMeetSucceeds(invocation1);

			TryMeetSucceeds(invocation2);

			TryMeetFails(invocation3);
		}


		[Test]
		public void MeetOrderedChildScope()
		{
			using (expectationScope.BeginOrdered())
			{
				AddExpectation(Expectation.ShouldMeetAtMost(1, invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeast(2, invocation2));
				AddExpectation(Expectation.ShouldMeetAtMost(3, invocation3));
			}


			TryMeetSucceeds(invocation1);
			
			TryMeetSucceeds(invocation2);
			TryMeetSucceeds(invocation2);
			TryMeetSucceeds(invocation2);

			TryMeetSucceeds(invocation3);
			TryMeetSucceeds(invocation3);
			TryMeetSucceeds(invocation3);


			AssertHasBeenMet();
		}

		[Test]
		public void FailToMeetOrderedChildScopeWrongToFewNumberOfInvocations()
		{
			using (expectationScope.BeginOrdered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeast(2, invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
			}


			TryMeetSucceeds(invocation1);

			TryMeetFails(invocation2);
		}

		[Test]
		public void FailToMeetOrderedChildScopeWrongToManyNumberOfInvocations()
		{
			using (expectationScope.BeginOrdered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeast(2, invocation2));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation3));
			}


			TryMeetSucceeds(invocation1);

			TryMeetSucceeds(invocation2);

			TryMeetFails(invocation3);
		}

		[Test]
		public void FailToMeetOrderedChildScopeWrongOrder()
		{
			using (expectationScope.BeginOrdered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation3));
			}


			TryMeetSucceeds(invocation1);

			TryMeetFails(invocation3);
		}

		[Test]
		public void FailToMeetOrderedChildScopeWrongInvocation()
		{
			using (expectationScope.BeginUnordered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
			}


			TryMeetSucceeds(invocation1);
						
			TryMeetSucceeds(invocation2);

			TryMeetFails(invocation3);
		}

		[Test]
		public void FailToMeetOrderedChildScopeMissingInvocations()
		{
			using (expectationScope.BeginUnordered())
			{
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation1));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation2));
				AddExpectation(Expectation.ShouldMeetAtLeastOnce(invocation3));
			}


			TryMeetSucceeds(invocation1);

			TryMeetSucceeds(invocation2);

			AssertHasNotBeenMet();
		}



		[Test]
		public void MustExitChildScopesInCorrectOrder()
		{
			var childScopeOuter = expectationScope.BeginOrdered();
			var childScopeInner = expectationScope.BeginUnordered();


		    Assert.Throws<InvalidOperationException>(() => childScopeOuter.Dispose());

			childScopeInner.Dispose();
			childScopeOuter.Dispose();
		}

		[Test]
		public void CantTryMeetNullInvocation()
		{
            Assert.Throws<ArgumentNullException>(() => ((IExpectationScope)expectationScope).TryMeet(null!, out var ignoredAction));
		}

		[Test]
		public void CantAddNullExpectation()
		{
		    Assert.Throws<ArgumentNullException>(() => ((IExpectationScope)expectationScope).Add(null!, false));
		}


		void TryMeetSucceeds(IInvocation invocation)
		{
			Assert.IsTrue(((IExpectationScope)expectationScope).TryMeet(invocation, out var ignoredAction));
		}

		void TryMeetFails(IInvocation invocation)
		{
            Assert.IsFalse(((IExpectationScope)expectationScope).TryMeet(invocation, out var ignoredAction));
		}

		void AssertHasBeenMet()
		{
			Assert.IsTrue(((IExpectation)expectationScope).HasBeenMet);
		}

		void AssertHasNotBeenMet()
		{
			Assert.IsFalse(((IExpectation)expectationScope).HasBeenMet);
		}

		void AddExpectation(IExpectation expectation)
		{
			((IExpectationScope)expectationScope).Add(expectation, false);
		}
		


		class Invocation : IInvocation
		{
			public IProxy Target => throw new NotSupportedException();
			public MethodInfo Method => throw new NotSupportedException();
			public IList<Type>? GenericArguments => throw new NotSupportedException();
			public IList<object?> ParameterValues => throw new NotSupportedException();
			public object? ReturnValue { set => throw new NotSupportedException(); }
		    public long InvocationOrder => throw new NotSupportedException();
		}

		class Expectation : IExpectation
		{
			IInvocation invocation;
			int expectedTimesMin = int.MinValue;
			int expectedTimesMax = int.MaxValue;
			int actualCount;

			public static IExpectation ShouldMeetAtLeastOnce(IInvocation invocation)
			{
				return ShouldMeetAtLeast(1, invocation);
			}

			public static IExpectation ShouldMeetAtLeast(int times, IInvocation invocation)
			{
				return new Expectation { invocation = invocation, expectedTimesMin = times };
			}

			public static IExpectation ShouldMeetAtMost(int times, IInvocation invocation)
			{
				return new Expectation { invocation = invocation, expectedTimesMax = times };
			}


			bool IExpectation.TryMeet(IInvocation invocation, out Action action)
			{
			    action = () => { };
				return (invocation == this.invocation && ++actualCount <= expectedTimesMax);
			}

			bool IExpectation.HasBeenMet
			{
				get { return actualCount >= expectedTimesMin && actualCount <= expectedTimesMax; }
			}
		}


	}
}
