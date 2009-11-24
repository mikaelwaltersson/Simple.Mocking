using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Actions;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
	public static class Expect
	{
		public static ISpecifyInvocation Once
		{
			get { return Exactly(1); }
		}

		public static ISpecifyInvocation AtLeastOnce
		{
			get { return AtLeast(1); }
		}

		public static ISpecifyInvocation AtMostOnce
		{
			get { return AtMost(1); }
		}

		public static ISpecifyInvocation Exactly(int times)
		{
			return BeginExpectationWithNumberOfInvocationsConstraint(times, times);
		}

		public static ISpecifyInvocation AtLeast(int times)
		{
			return BeginExpectationWithNumberOfInvocationsConstraint(times, null);
		}

		public static ISpecifyInvocation AtMost(int times)
		{
			return BeginExpectationWithNumberOfInvocationsConstraint(null, times);
		}

		public static ISpecifyInvocation Between(int fromInclusive, int toInclusive)
		{
			return BeginExpectationWithNumberOfInvocationsConstraint(fromInclusive, toInclusive);
		}


		public static ISpecifyAction MethodCall(Expression<Action> methodCallExpression)
		{
			return BeginExpectation().MethodCall(methodCallExpression);
		}

		public static ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression)
		{
			return BeginExpectation().MethodCall(methodCallExpression);
		}

		public static ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression)
		{
			return BeginExpectation().PropertyGet(propertyExpression);
		}

		public static ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, T value)
		{
			return BeginExpectation().PropertySet(propertyExpression, value);
		}

		public static ISpecifyAction EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler)
		{
			return BeginExpectation().EventAdd(target, eventName, handler);
		}

		public static ISpecifyAction EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler)
		{
			return BeginExpectation().EventRemove(target, eventName, handler);
		}

		public static void AnyInvocationOn(object target)
		{
			CreateExpectation(InvocationMatcher.ForAnyInvocationOn(target), new NumberOfInvocationsConstraint(null, null));
		}

		static ISpecifyInvocation BeginExpectation()
		{
			return BeginExpectationWithNumberOfInvocationsConstraint(null, null);
		}

		static ISpecifyInvocation BeginExpectationWithNumberOfInvocationsConstraint(int? fromInclusive, int? toInclusive)
		{
			return new SpecifyInvocation(new NumberOfInvocationsConstraint(fromInclusive, toInclusive));
		}

		static Expectation CreateExpectation(InvocationMatcher invocationMatcher, NumberOfInvocationsConstraint numberOfInvocationsConstraint)
		{
			var expectation = new Expectation(invocationMatcher, numberOfInvocationsConstraint);

			MockInvocationInterceptor.GetFromTarget(invocationMatcher.Target).AddExpectation(expectation);

			return expectation;
		}

		
		class SpecifyInvocation : ISpecifyInvocation
		{
			NumberOfInvocationsConstraint numberOfInvocationsConstraint;

			public SpecifyInvocation(NumberOfInvocationsConstraint numberOfInvocationsConstraint)
			{
				this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
			}

			ISpecifyAction ISpecifyInvocation.MethodCall(Expression<Action> methodCallExpression)
			{
				return ActionInvoked(InvocationMatcher.ForMethodCall(methodCallExpression));
			}

			ISpecifyAction<T> ISpecifyInvocation.MethodCall<T>(Expression<Func<T>> methodCallExpression)
			{
				return ActionInvoked<T>(InvocationMatcher.ForMethodCall(methodCallExpression));
			}

			ISpecifyAction<T> ISpecifyInvocation.PropertyGet<T>(Expression<Func<T>> propertyExpression)
			{
				return ActionInvoked<T>(InvocationMatcher.ForPropertyGet(propertyExpression));
			}

			ISpecifyAction ISpecifyInvocation.PropertySet<T>(Expression<Func<T>> propertyExpression, T value)
			{
				return ActionInvoked(InvocationMatcher.ForPropertySet(propertyExpression, value));
			}

			ISpecifyAction ISpecifyInvocation.EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler)
			{
				return ActionInvoked(InvocationMatcher.ForEventAdd(target, eventName, handler));
			}

			ISpecifyAction ISpecifyInvocation.EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler)
			{
				return ActionInvoked(InvocationMatcher.ForEventRemove(target, eventName, handler));
			}

			ISpecifyAction ActionInvoked(InvocationMatcher invocationMatcher)
			{
				return new SpecifyAction(CreateExpectation(invocationMatcher, numberOfInvocationsConstraint));
			}

			ISpecifyAction<T> ActionInvoked<T>(InvocationMatcher invocationMatcher)
			{
				return new SpecifyAction<T>(CreateExpectation(invocationMatcher, numberOfInvocationsConstraint));
			}		
		}

		abstract class SpecifyActionBase
		{
			Expectation expectation;

			protected SpecifyActionBase(Expectation expectation)
			{
				this.expectation = expectation;
			}

			protected void AppendExecutesAction(Delegate actionOrFunc)
			{
				AppendAction(new ExecutesAction(actionOrFunc));
			}

			protected void AppendReturnsAction(object value)
			{
				AppendAction(new ReturnsAction(value));
			}

			protected void AppendThrowsAction(Exception ex)
			{
				AppendAction(new ThrowsAction(ex));
			}

			protected void AppendSetsOutOrRefParameterAction(int index, object value)
			{
				AppendAction(new SetsOutOrRefParameterAction(index, value));
			}

			void AppendAction(IAction action)
			{
				expectation.AddAction(action);
			}
		}

		class SpecifyAction : SpecifyActionBase, ISpecifyAction
		{
			public SpecifyAction(Expectation expectation)
				: base(expectation)
			{
			}

			void ISpecifyAction.Executes(Action action)
			{
				AppendExecutesAction(action);
			}

			void ISpecifyAction.Executes(ActionWithParameters action)
			{
				AppendExecutesAction(action);
			}

			void ISpecifyAction.Throws(Exception ex)
			{
				AppendThrowsAction(ex);
			}

			ISpecifyAction ISpecifyAction.SetsOutOrRefParameter(int index, object value)
			{
				AppendSetsOutOrRefParameterAction(index, value);
				return this;
			}
		}

		class SpecifyAction<T> : SpecifyActionBase, ISpecifyAction<T>
		{
			public SpecifyAction(Expectation expectation)
				: base(expectation)
			{
			}

			void ISpecifyAction<T>.Executes(Func<T> func)
			{
				AppendExecutesAction(func);
			}

			void ISpecifyAction<T>.Executes(FuncWithParameters<T> func)
			{
				AppendExecutesAction(func);
			}

			void ISpecifyAction<T>.Returns(T value)
			{
				AppendReturnsAction(value);
			}

			void ISpecifyAction<T>.Throws(Exception ex)
			{
				AppendThrowsAction(ex);
			}

			ISpecifyAction<T> ISpecifyAction<T>.SetsOutOrRefParameter(int index, object value)
			{
				AppendSetsOutOrRefParameterAction(index, value);
				return this;
			}
		}

	}
}
