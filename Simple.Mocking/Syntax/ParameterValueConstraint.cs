using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.Syntax
{
	public abstract class ParameterValueConstraint<T> : IParameterValueConstraint
	{
		public T AsRefOrOut;

		public static implicit operator T(ParameterValueConstraint<T> value)
		{
			throw new InvalidOperationException();
		}
		
		bool IParameterValueConstraint.Matches(object value)
		{
			return (typeof(T).IsAssignable(value) && Matches((T)value));
		}

		protected abstract bool Matches(T value);
	}


}
