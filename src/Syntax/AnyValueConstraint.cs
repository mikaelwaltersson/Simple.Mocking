using System;
using System.Linq.Expressions;

namespace Simple.Mocking.Syntax
{
    public sealed class AnyValueConstraint<T> : ParameterValueConstraint<T>
	{
		public ParameterValueConstraint<T> Matching(Expression<Func<T, bool>> predicateExpression) => 
			new MatchingPredicateValueConstraint<T>(predicateExpression);

		public override string ToString() => string.Format("Any<{0}>.Value", typeof(T).Name);

		protected override bool Matches(T value) => true;
	}
}