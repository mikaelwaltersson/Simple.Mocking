namespace Simple.Mocking.SetUp
{
    sealed class NumberOfInvocationsConstraint
	{
		int? fromInclusive;
		int? toInclusive;

	    public NumberOfInvocationsConstraint(int? fromInclusive, int? toInclusive)
		{
			this.fromInclusive = fromInclusive;
			this.toInclusive = toInclusive;
		}

		public bool Matches(int invocationCount) =>
			(MatchesLowerBound(invocationCount) && MatchesUpperBound(invocationCount));

		bool MatchesLowerBound(int invocationCount) =>
			(!fromInclusive.HasValue || invocationCount >= fromInclusive.Value);

		bool MatchesUpperBound(int invocationCount) => 
			(!toInclusive.HasValue || invocationCount <= toInclusive.Value);

		public bool CanInvoke(int currentInvocationCount) =>
			MatchesUpperBound(currentInvocationCount + 1);

		public override string ToString() =>
			(toInclusive == fromInclusive ? ToString(toInclusive) : (ToString(fromInclusive) + ".." + ToString(toInclusive)));

		string ToString(int? bound) =>
			(bound.HasValue ? bound.Value.ToString() : "*");
	}
}
