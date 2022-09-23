using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;

namespace Simple.Mocking.Asserts
{
    class AssertInvocations : IAssertInvocations
    {
        MatchedInvocations? previousMatch;

        public AssertInvocations(MatchedInvocations? previousMatch)
        {
            this.previousMatch = previousMatch;
        }

        public IAssertInvocationFor None => Exactly(0);

        public IAssertInvocationFor Once => Exactly(1);

        public IAssertInvocationFor AtLeastOnce => AtLeast(1);
    
        public IAssertInvocationFor AtMostOnce => AtMost(1);

        public IAssertInvocationFor Exactly(int times) => ConstrainNumberOfInvocations(times, times);

        public IAssertInvocationFor AtLeast(int times) => ConstrainNumberOfInvocations(times, null);

        public IAssertInvocationFor AtMost(int times) => ConstrainNumberOfInvocations(null, times);

        public IAssertInvocationFor Between(int fromInclusive, int toInclusive) => ConstrainNumberOfInvocations(fromInclusive, toInclusive);

        public void InOrderAsSpecified() => MatchedInvocations.AssertInvocationsWasMadeInSpecifiedOrder(previousMatch);

        IAssertInvocationFor ConstrainNumberOfInvocations(int? fromInclusive, int? toInclusive) =>
            new AssertInvocationFor(previousMatch, new NumberOfInvocationsConstraint(fromInclusive, toInclusive));
    }
}
