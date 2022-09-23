using System;

namespace Simple.Mocking.SetUp.Proxies
{
    public interface IProxy
	{
		Type ProxiedType { get; }
		object BaseObject { get; }
		IInvocationInterceptor InvocationInterceptor { get; }		
	}
}