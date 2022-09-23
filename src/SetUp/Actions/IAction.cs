using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.SetUp.Actions
{
    interface IAction
	{
		void ExecuteFor(IInvocation invocation);
	}
}
