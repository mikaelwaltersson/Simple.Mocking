using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
    public static class Any<T>
	{
		public static readonly AnyValueConstraint<T> Value = new AnyValueConstraint<T>();
	}
}
