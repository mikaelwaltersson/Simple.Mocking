using System;
using System.Linq.Expressions;

namespace Simple.Mocking.Syntax
{
    public sealed class MatchingPredicateValueConstraint<T> : ParameterValueConstraint<T>
	{
		Expression<Func<T, bool>> predicateExpression;
		Func<T, bool> predicate;

		public MatchingPredicateValueConstraint(Expression<Func<T, bool>> predicateExpression)
		{
			if (predicateExpression == null)
				throw new ArgumentNullException("predicateExpression");

			this.predicateExpression = predicateExpression;
			this.predicate = predicateExpression.Compile();
		}

		public override string ToString() => string.Format("Any<{0}>.Value.Matching({1})", typeof(T).Name, predicateExpression);

		protected override bool Matches(T value) => predicate(value);
	}
}