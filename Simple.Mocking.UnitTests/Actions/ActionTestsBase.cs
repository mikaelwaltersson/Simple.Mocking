using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp.Proxies;
using Simple.Mocking.UnitTests.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.Actions
{
	public abstract class ActionTestsBase
	{
		protected Invocation CreateInvocation()
		{
			return new Invocation(target, typeof(Target).GetMethod("Method"), null, new object[2], null);
		}

		Target target = new Target();

		class Target : ProxyBase<object>
		{
			public Target() 
				: base(string.Empty, new TestInvocationInterceptor())
			{
			}

			public object Method(ref object value)
			{
				value = null;
				return null;
			}
		}
	}
}