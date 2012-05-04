using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public bool Matches(int invocationCount)
		{
			return (MatchesLowerBound(invocationCount) && MatchesUpperBound(invocationCount));
		}

		bool MatchesLowerBound(int invocationCount)
		{
			return (!fromInclusive.HasValue || invocationCount >= fromInclusive.Value);
		}

		bool MatchesUpperBound(int invocationCount)
		{
			return (!toInclusive.HasValue || invocationCount <= toInclusive.Value);
		}

		public bool CanInvoke(int currentInvocationCount)
		{
			return MatchesUpperBound(currentInvocationCount + 1);
		}

		public override string ToString()
		{
			return (toInclusive == fromInclusive ? ToString(toInclusive) : (ToString(fromInclusive) + ".." + ToString(toInclusive)));
		}

		string ToString(int? bound)
		{
			return (bound.HasValue ? bound.Value.ToString() : "*");
		}
	}
}
