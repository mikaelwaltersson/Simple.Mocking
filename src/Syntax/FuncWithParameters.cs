using System.Collections.Generic;

namespace Simple.Mocking.Syntax
{
    public delegate T FuncWithParameters<T>(IList<object?> parameters);
}