namespace Simple.Mocking.SetUp
{
    interface IParameterValueConstraint
	{
		bool Matches(object? value);
	}
}