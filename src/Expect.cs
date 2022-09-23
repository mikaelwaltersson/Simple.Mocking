using System;
using System.Linq.Expressions;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Actions;
using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public static class Expect
	{
		public static ISpecifyInvocationWithPrecedence Once => Exactly(1);

		public static ISpecifyInvocationWithPrecedence AtLeastOnce => AtLeast(1);
		
		public static ISpecifyInvocationWithPrecedence AtMostOnce => AtMost(1);

		public static ISpecifyInvocationWithPrecedence Exactly(int times) =>
			BeginExpectationWithNumberOfInvocationsConstraint(times, times);

		public static ISpecifyInvocationWithPrecedence AtLeast(int times) =>
			BeginExpectationWithNumberOfInvocationsConstraint(times, null);

		public static ISpecifyInvocationWithPrecedence AtMost(int times) =>
			BeginExpectationWithNumberOfInvocationsConstraint(null, times);

		public static ISpecifyInvocationWithPrecedence Between(int fromInclusive, int toInclusive) =>
			BeginExpectationWithNumberOfInvocationsConstraint(fromInclusive, toInclusive);

		public static ISpecifyAction MethodCall(Expression<Action> methodCallExpression) =>
			BeginExpectation().MethodCall(methodCallExpression);

		public static ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression) =>
			BeginExpectation().MethodCall(methodCallExpression);

		public static ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression) =>
			BeginExpectation().PropertyGet(propertyExpression);

		public static ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, T value) =>
			BeginExpectation().PropertySet(propertyExpression, value);

        public static ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, ParameterValueConstraint<T> value) =>
			BeginExpectation().PropertySet(propertyExpression, value);

		public static ISpecifyAction EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler) where TTarget : notnull =>
			BeginExpectation().EventAdd(target, eventName, handler);

		public static ISpecifyAction EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler) where TTarget : notnull =>
			BeginExpectation().EventRemove(target, eventName, handler);

        public static ISpecifyActionForAny AnyInvocationOn(object target) =>
        	BeginExpectation().AnyInvocationOn(target);

		public static ISpecifyInvocation WithHigherPrecedence =>
			BeginExpectation().WithHigherPrecedence;

		static SpecifyInvocation BeginExpectation() =>
			BeginExpectationWithNumberOfInvocationsConstraint(null, null);

		static SpecifyInvocation BeginExpectationWithNumberOfInvocationsConstraint(int? fromInclusive, int? toInclusive) =>
			new SpecifyInvocation(new NumberOfInvocationsConstraint(fromInclusive, toInclusive));

		static Expectation CreateExpectation(InvocationMatcher invocationMatcher, NumberOfInvocationsConstraint numberOfInvocationsConstraint, bool hasHigherPrecedence)
		{
			var expectation = new Expectation(invocationMatcher, numberOfInvocationsConstraint);

			MockInvocationInterceptor.GetFromTarget(invocationMatcher.Target).AddExpectation(expectation, hasHigherPrecedence);

			return expectation;
		}

		class SpecifyInvocation : ISpecifyInvocationWithPrecedence
		{
			NumberOfInvocationsConstraint numberOfInvocationsConstraint;
			bool hasHigherPrecedence;

			public SpecifyInvocation(NumberOfInvocationsConstraint numberOfInvocationsConstraint)
			{
				this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
			}

            public ISpecifyAction MethodCall(Expression<Action> methodCallExpression) =>
				ActionInvoked(InvocationMatcher.ForMethodCall(methodCallExpression));

			public ISpecifyAction<T> MethodCall<T>(Expression<Func<T>> methodCallExpression) =>
				ActionInvoked<T>(InvocationMatcher.ForMethodCall(methodCallExpression));

            public ISpecifyAction<T> PropertyGet<T>(Expression<Func<T>> propertyExpression) =>
				ActionInvoked<T>(InvocationMatcher.ForPropertyGet(propertyExpression));

            public ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, T value) =>
				ActionInvoked(InvocationMatcher.ForPropertySet(propertyExpression, value));

            public ISpecifyAction PropertySet<T>(Expression<Func<T>> propertyExpression, ParameterValueConstraint<T> value) =>
            	ActionInvoked(InvocationMatcher.ForPropertySet(propertyExpression, value));

            public ISpecifyAction EventAdd<TTarget, THandler>(TTarget target, string eventName, THandler handler) where TTarget : notnull =>
				ActionInvoked(InvocationMatcher.ForEventAdd(target, eventName, handler));

            public ISpecifyAction EventRemove<TTarget, THandler>(TTarget target, string eventName, THandler handler) where TTarget : notnull =>
				ActionInvoked(InvocationMatcher.ForEventRemove(target, eventName, handler));

            public ISpecifyInvocation WithHigherPrecedence =>
            	new SpecifyInvocation(numberOfInvocationsConstraint) { hasHigherPrecedence = true };

            public ISpecifyActionForAny AnyInvocationOn(object target) =>
            	ActionInvoked(InvocationMatcher.ForAnyInvocationOn(target));

            SpecifyAction ActionInvoked(InvocationMatcher invocationMatcher) =>
				new SpecifyAction(CreateExpectation(invocationMatcher, numberOfInvocationsConstraint, hasHigherPrecedence));

            SpecifyAction<T> ActionInvoked<T>(InvocationMatcher invocationMatcher) =>
				new SpecifyAction<T>(CreateExpectation(invocationMatcher, numberOfInvocationsConstraint, hasHigherPrecedence));
		}

		abstract class SpecifyActionBase
		{
			Expectation expectation;

			protected SpecifyActionBase(Expectation expectation)
			{
				this.expectation = expectation;
			}

			protected void AppendExecutesAction(Delegate actionOrFunc) =>
				AppendAction(new ExecutesAction(actionOrFunc));

			protected void AppendReturnsAction(object? value) =>
				AppendAction(new ReturnsAction(value));

			protected void AppendThrowsAction(Exception ex) =>
				AppendAction(new ThrowsAction(ex));

			protected void AppendSetsOutOrRefParameterAction(int index, object? value) =>
				AppendAction(new SetsOutOrRefParameterAction(index, value));

		    protected void AppendAction(IAction action) =>
				expectation.AddAction(action);
		}

		class SpecifyAction : SpecifyActionBase, ISpecifyAction, ISpecifyActionForAny
		{
			public SpecifyAction(Expectation expectation)
				: base(expectation)
			{
			}

            public void Executes(Action action) =>
				AppendExecutesAction(action);

            public void Executes(ActionWithParameters action) =>
				AppendExecutesAction(action);

            public void Throws(Exception ex) =>
				AppendThrowsAction(ex);

            public ISpecifyAction SetsOutOrRefParameter(int index, object? value)
			{
				AppendSetsOutOrRefParameterAction(index, value);
				return this;
			}

            public ISpecifyActionForAny SetsOutOrRefParameters(Func<Type, object?> valueForType)
            {
                AppendAction(new SetsOutOrRefParametersForAnyAction(valueForType));
                return this;
            }

            public void Returns(Func<Type, object?> valueForType) =>
                AppendAction(new ReturnsForAnyAction(valueForType));
		}

		class SpecifyAction<T> : SpecifyActionBase, ISpecifyAction<T>
		{
			public SpecifyAction(Expectation expectation)
				: base(expectation)
			{
			}

			void ISpecifyAction<T>.Executes(Func<T> func) =>
				AppendExecutesAction(func);

            void ISpecifyAction<T>.Executes(FuncWithParameters<T> func) =>
				AppendExecutesAction(func);

            void ISpecifyAction<T>.Returns(T value) =>
				AppendReturnsAction(value);

			void ISpecifyAction<T>.Throws(Exception ex) =>
				AppendThrowsAction(ex);

            ISpecifyAction<T> ISpecifyAction<T>.SetsOutOrRefParameter(int index, object? value)
			{
				AppendSetsOutOrRefParameterAction(index, value);
				return this;
			}
		}
	}
}
