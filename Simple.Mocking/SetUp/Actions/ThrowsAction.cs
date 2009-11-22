using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class ThrowsAction : IAction
	{
		Exception exception;

		public ThrowsAction(Exception exception)
		{
			this.exception = exception;
		}

		public void ExecuteFor(IInvocation invocation)
		{
			throw exception;
		}
	}
}