using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
	class SetsOutOrRefParametersForAnyAction : ActionForAnyBase
	{
        public SetsOutOrRefParametersForAnyAction(Func<Type, object> valueForType)
            : base(valueForType)
		{
		}

	    protected override void ExecuteFor(MethodInfo nonGenericMethod, IInvocation invocation)
	    {
            var parameters = nonGenericMethod.GetParameters();

	        for (var i = 0; i < parameters.Length; i++)
	        {
	            var parameterType = parameters[i].ParameterType;

                if (parameterType.IsByRef)
                    invocation.ParameterValues[i] = GetValueForType(parameterType.GetRealTypeForByRefType());   	                                
	        }
	    }
	}
}