using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class ReturnsForAnyAction : ActionForAnyBase
	{
        public ReturnsForAnyAction(Func<Type, object> valueForType)
            : base(valueForType)
		{
		}


	    protected override void ExecuteFor(MethodInfo nonGenericMethod, IInvocation invocation)
	    {
	        var returnType = nonGenericMethod.ReturnType;
     
            if (returnType != typeof(void))
                invocation.ReturnValue = GetValueForType(returnType);
	    }
	}
}