using System;

namespace Simple.Mocking.SetUp
{
    static class TypeParameter
	{
		public static bool IsDelegateType(this Type type)
		{
			return typeof(Delegate).IsAssignableFrom(type);
		}

		public static bool IsAssignable(this Type type, object? value)
		{
			return (value == null ? !type.IsValueType : type.IsAssignableFrom(value.GetType()));
		}
	}
}