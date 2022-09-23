namespace Simple.Mocking.SetUp.Proxies
{
    public interface IInvocationInterceptor
	{
		void OnInvocation(IInvocation invocation);
	}
}