namespace Simple.Mocking.Syntax
{
    public interface ISpecifyInvocationWithPrecedence : ISpecifyInvocation
	{
		ISpecifyInvocation WithHigherPrecedence { get; }	    
	}
}
