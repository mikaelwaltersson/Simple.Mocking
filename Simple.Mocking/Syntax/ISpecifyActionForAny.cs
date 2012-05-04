using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.Syntax
{
    public interface ISpecifyActionForAny
    {        
        void Throws(Exception ex);

        void Returns(Func<Type, object> valueForType);

        ISpecifyActionForAny SetsOutOrRefParameters(Func<Type, object> valueForType);
    }
}