using System;
using System.Collections.Generic;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
    class ExecutesAction : IAction
	{
		Delegate actionOrFunc;

		public ExecutesAction(Delegate actionOrFunc)
		{
			this.actionOrFunc = actionOrFunc;				
		}

		public void ExecuteFor(IInvocation invocation)
		{
            var parameters = HasParametersArgument ? new object?[] { invocation.ParameterValues } : new object?[0];					
			var returnValue = actionOrFunc.DynamicInvoke(parameters);

			if (ExpectsReturnValue(invocation))
				invocation.ReturnValue = returnValue;
		}

	    bool ExpectsReturnValue(IInvocation invocation) => 
			Invocation.GetNonGenericMethod(invocation).ReturnType != typeof(void);

        bool HasParametersArgument
        {
            get
            {
                var parameters = actionOrFunc.Method.GetParameters();

                return (parameters.Length == 1 && typeof(IList<object>).IsAssignableFrom(parameters[0].ParameterType));
            }
        }
	}
}