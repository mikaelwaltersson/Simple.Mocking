using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking.Asserts
{
    class AssertInvocations : IAssertInvocations
    {
        MatchedInvocations previousMatch;


        public AssertInvocations(MatchedInvocations previousMatch)
        {
            this.previousMatch = previousMatch;
        }

        public IAssertInvocationFor None
        {
            get { return Exactly(0); }
        }

        public IAssertInvocationFor Once
        {
            get { return Exactly(1); }
        }

        public IAssertInvocationFor AtLeastOnce
        {
            get { return AtLeast(1); }
        }

        public IAssertInvocationFor AtMostOnce
        {
            get { return AtMost(1); }
        }

        public IAssertInvocationFor Exactly(int times)
        {
            return ConstrainNumberOfInvocations(times, times);
        }

        public IAssertInvocationFor AtLeast(int times)
        {
            return ConstrainNumberOfInvocations(times, null);
        }

        public IAssertInvocationFor AtMost(int times)
        {
            return ConstrainNumberOfInvocations(null, times);
        }

        public IAssertInvocationFor Between(int fromInclusive, int toInclusive)
        {
            return ConstrainNumberOfInvocations(fromInclusive, toInclusive);
        }

        public void InOrderAsSpecified()
        {
            previousMatch.AssertInvocationsWasMadeInSpecifiedOrder();
        }

        IAssertInvocationFor ConstrainNumberOfInvocations(int? fromInclusive, int? toInclusive)
        {
            return new AssertInvocationFor(previousMatch, new NumberOfInvocationsConstraint(fromInclusive, toInclusive));
        }

    }

}
