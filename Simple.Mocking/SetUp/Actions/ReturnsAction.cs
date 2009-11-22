using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class ReturnsAction : IAction
	{
		object value;

		public ReturnsAction(object value)
		{
			this.value = value;
		}

		public void ExecuteFor(IInvocation invocation)
		{
			invocation.ReturnValue = value;
		}
	}
}