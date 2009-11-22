using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class SetsOutOrRefParameterAction : IAction
	{
		int index;
		object value;

		public SetsOutOrRefParameterAction(int index, object value)
		{
			this.index = index;
			this.value = value;
		}

		public void ExecuteFor(IInvocation invocation)
		{
			invocation.ParameterValues[index] = value;
		}
	}
}