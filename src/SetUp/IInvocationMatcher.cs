using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp
{
    interface IInvocationMatcher
	{
		bool Matches(IInvocation invocation);
	}
}