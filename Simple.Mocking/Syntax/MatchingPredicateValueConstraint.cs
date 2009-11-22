using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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

		public override string ToString()
		{
			return string.Format("Any<{0}>.Value.Matching({1})", typeof(T).Name, predicateExpression);
		}

		protected override bool Matches(T value)
		{
			return predicate(value);
		}
	}
}