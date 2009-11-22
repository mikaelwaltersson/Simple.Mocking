using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class ExecutesAction : IAction
	{
		Delegate actionOrFunc;
		bool hasReturnValue;
		bool hasParametersArgument;

		public ExecutesAction(Delegate actionOrFunc)
		{
			this.actionOrFunc = actionOrFunc;
			this.hasReturnValue = (actionOrFunc.Method.ReturnType != typeof(void));
			this.hasParametersArgument = 
				((from parameter in actionOrFunc.Method.GetParameters() select parameter.ParameterType).SequenceEqual(new[] { typeof(IList<object>) }));
		}

		public void ExecuteFor(IInvocation invocation)
		{
			var parameters = (hasParametersArgument ? new object[] { invocation.ParameterValues } : new object[0]);					
			var returnValue = actionOrFunc.DynamicInvoke(parameters);


			if (hasReturnValue)
				invocation.ReturnValue = returnValue;
		}
	}
}