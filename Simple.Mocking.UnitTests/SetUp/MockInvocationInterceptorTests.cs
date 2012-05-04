using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp
{
	[TestFixture]
	public class MockInvocationInterceptorTests
	{
		[Test]
		public void AddExpectation()
		{
			var invocationInterceptor = new MockInvocationInterceptor(new TestExpectationScope());
			
			var expectation = new TestExpectation();
			invocationInterceptor.AddExpectation(expectation, false);

			Assert.AreSame(expectation, ((TestExpectationScope)invocationInterceptor.ExpectationScope).AddedExpectation);
		}

		[Test]
		public void OnInvocation()
		{
			var expectationScope = new TestExpectationScope();
			var invocationInterceptor = new MockInvocationInterceptor(expectationScope);
			var target = new Target(invocationInterceptor);
			var invocation = CreateMethodInvocation<IExpectationScope>(target, "Add", new[] { typeof(IExpectation), typeof(bool) }, new object[2]);


            Assert.Throws<ExpectationsException>(() => invocationInterceptor.OnInvocation(invocation));


			expectationScope.CanMeet = true;		
			invocationInterceptor.OnInvocation(invocation);

			Assert.IsTrue(expectationScope.HasBeenMet);
		}

	

		[Test]
		public void OnInvocationForObjectMethod()
		{
			var baseObject = new BaseObject();

			var expectationScope = new TestExpectationScope();
			var invocationInterceptor = new MockInvocationInterceptor(expectationScope);
			var target = new Target(baseObject, invocationInterceptor);
			
			var invocation = CreateMethodInvocation<object>(target, "ToString");
			int invocationCount = 0;

			baseObject.ToStringCallback = () => (++invocationCount).ToString();

			
			invocationInterceptor.OnInvocation(invocation);
		
			Assert.AreEqual("1", invocation.ReturnValue);
			Assert.AreEqual(1, invocationCount);
		}

		[Test]
		public void GetFromTarget()
		{
			var invocationInterceptor = new MockInvocationInterceptor(new TestExpectationScope());
			var target = new Target(invocationInterceptor);

			Assert.AreSame(invocationInterceptor, MockInvocationInterceptor.GetFromTarget(target));
		}

		[Test]
		public void GetFromTargetNonProxy()
		{
            Assert.Throws<ArgumentException>(() => MockInvocationInterceptor.GetFromTarget(new object()));
		}

		[Test]
		public void GetFromTargetProxyWithNonMockInvocationInterceptor()
		{
            Assert.Throws<ArgumentException>(() => MockInvocationInterceptor.GetFromTarget(new Target(new OtherInvocationInterceptor())));
		}

		[Test]
		public void CantCreateInvocationInterceptorWithNullArguments()
		{
            Assert.Throws<ArgumentNullException>(() => new MockInvocationInterceptor(null));
		}

		[Test]
		public void CantInvokeWithNullInvocation()
		{
            Assert.Throws<ArgumentNullException>(() => new MockInvocationInterceptor(new TestExpectationScope()).OnInvocation(null));
		}

		[Test]
		public void CantAddNullExpectation()
		{
            Assert.Throws<ArgumentNullException>(() => new MockInvocationInterceptor(new TestExpectationScope()).AddExpectation(null, false));
		}


		Invocation CreateMethodInvocation<T>(IProxy target, string methodName, params object[] parameterValues)
		{
			return CreateMethodInvocation<T>(target, methodName, parameterValues.Select(value => value.GetType()).ToArray(), parameterValues);
		}

		Invocation CreateMethodInvocation<T>(IProxy target, string methodName, Type[] parameterTypes, object[] parameterValues)
		{
			return new Invocation(target, typeof(T).GetMethod(methodName, parameterTypes), null, parameterValues, null, 0);
		}


		class BaseObject
		{
			public Func<string> ToStringCallback;

			public override string ToString()
			{
				return ToStringCallback();
			}
		}

		class Target : ProxyBase<object>
		{
			public Target(IInvocationInterceptor invocationInterceptor)
				: base(new object(), invocationInterceptor)
			{
			}

			public Target(object baseObject, IInvocationInterceptor invocationInterceptor)
				: base(baseObject, invocationInterceptor)
			{
			}
		}

		class OtherInvocationInterceptor : IInvocationInterceptor
		{
			public void OnInvocation(IInvocation invocation)
			{				
			}
		}

		class TestExpectation : IExpectation
		{
			public bool CanMeet;
	
			public bool TryMeet(IInvocation invocation, out Action action)
			{
			    action = () => { };
				return (HasBeenMet = CanMeet);
			}

			public bool HasBeenMet { get; private set; }
		}

		class TestExpectationScope : TestExpectation, IExpectationScope, IInvocationHistory
		{
			public IExpectation AddedExpectation;

			public void Add(IExpectation expectation, bool hasHigherPrecedence)
			{
				AddedExpectation = expectation;
			}

			public IInvocationHistory InvocationHistory
			{
				get { return this; }
			}

			public void RegisterInvocation(IInvocation invocation, bool wasExpected)
			{				
			}

            public IEnumerable<IInvocation> Invocations
            {
                get { return new IInvocation[0]; }
            }

			public IEnumerable<IInvocation> UnexpectedInvocations
			{
				get { return new IInvocation[0]; }
			}
		}

	}
}