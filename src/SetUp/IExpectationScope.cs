namespace Simple.Mocking.SetUp
{
    interface IExpectationScope : IExpectation
	{
		void Add(IExpectation expectation, bool hasHigherPrecedence);

		IInvocationHistory InvocationHistory { get; }
	}
}