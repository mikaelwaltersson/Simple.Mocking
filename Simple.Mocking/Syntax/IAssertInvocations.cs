using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.Syntax
{
    public interface IAssertInvocations
    {
        IAssertInvocationFor None { get; }

        IAssertInvocationFor Once { get; }

        IAssertInvocationFor AtLeastOnce { get; }

        IAssertInvocationFor AtMostOnce { get; }
        
        IAssertInvocationFor Exactly(int times);

        IAssertInvocationFor AtLeast(int times);

        IAssertInvocationFor AtMost(int times);

        IAssertInvocationFor Between(int fromInclusive, int toInclusive);

        void InOrderAsSpecified();

    }
}
