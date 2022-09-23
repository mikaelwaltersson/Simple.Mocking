using System;
using System.Collections.Generic;

using Simple.Mocking.SetUp.Actions;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
    class Expectation : IExpectation
	{				
		IInvocationMatcher invocationMatcher;
		NumberOfInvocationsConstraint numberOfInvocationsConstraint;
		List<IAction> actions;
		int invocationCount;


		public Expectation(IInvocationMatcher invocationMatcher, NumberOfInvocationsConstraint numberOfInvocationsConstraint)
		{
			if (invocationMatcher == null)
				throw new ArgumentNullException("invocationMatcher");

			if (numberOfInvocationsConstraint == null)
				throw new ArgumentNullException("numberOfInvocationsConstraint");

			this.invocationMatcher = invocationMatcher;
			this.numberOfInvocationsConstraint = numberOfInvocationsConstraint;
			this.actions = new List<IAction>();
		}

		public bool TryMeet(IInvocation invocation, out Action? action)
		{
			if (numberOfInvocationsConstraint.CanInvoke(invocationCount) && invocationMatcher.Matches(invocation))
			{
				invocationCount++;
                action = () => ExecuteActionsFor(invocation);				
				return true;
			}

		    action = null;
			return false;
		}

        void ExecuteActionsFor(IInvocation invocation) =>
        	actions.ForEach(action => action.ExecuteFor(invocation));

	    public bool HasBeenMet => 
			numberOfInvocationsConstraint.Matches(invocationCount);

		public void AddAction(IAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			actions.Add(action);
		}

		public override string ToString() =>
			string.Format(
				"(invoked: {0} of {1}) {2}", 
				invocationCount, numberOfInvocationsConstraint, invocationMatcher);
	}
}
