using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.Syntax
{
	public interface ISpecifyInvocationWithPrecedence : ISpecifyInvocation
	{
		ISpecifyInvocation WithHigherPrecedence { get; }	    
	}


}
