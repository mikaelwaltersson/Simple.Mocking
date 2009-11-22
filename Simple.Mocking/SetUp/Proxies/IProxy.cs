using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	public interface IProxy
	{
		Type ProxiedType { get; }
		object BaseObject { get; }
		IInvocationInterceptor InvocationInterceptor { get; }		
	}
}