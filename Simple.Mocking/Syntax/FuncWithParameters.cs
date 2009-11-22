using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.Syntax
{
	public delegate T FuncWithParameters<T>(IList<object> parameters);
}