using System;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.Syntax
{
    public abstract class ParameterValueConstraint<T> : IParameterValueConstraint
	{
#pragma warning disable CS8618
		public T AsRefOrOut;
		public T AsInterface;
#pragma warning disable CS8618

		public static implicit operator T(ParameterValueConstraint<T> value) =>
			throw new InvalidOperationException();
		
		bool IParameterValueConstraint.Matches(object? value) =>
			(typeof(T).IsAssignable(value) && Matches((T)value!));

		protected abstract bool Matches(T value);
	}
}
