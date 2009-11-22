using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Mocking.Syntax
{
	public sealed class AnyValueConstraint<T> : ParameterValueConstraint<T>
	{
		public ParameterValueConstraint<T> Matching(Expression<Func<T, bool>> predicateExpression)
		{
			return new MatchingPredicateValueConstraint<T>(predicateExpression);
		}

		public override string ToString()
		{
			return string.Format("Any<{0}>.Value", typeof(T).Name);
		}

		protected override bool Matches(T value)
		{
			return true;
		}

	}
}