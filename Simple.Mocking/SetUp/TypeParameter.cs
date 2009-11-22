using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
	static class TypeParameter
	{
		const string ByRefSpecifier = "&";

		public static Type GetRealTypeForByRefType(this Type type)
		{
			if (!type.IsByRef)
				throw new InvalidOperationException();

			return Type.GetType(type.AssemblyQualifiedName.Replace(ByRefSpecifier, string.Empty), true);
		}

		public static bool IsDelegateType(this Type type)
		{
			return typeof(Delegate).IsAssignableFrom(type);
		}

		public static bool IsAssignable(this Type type, object value)
		{
			return (value == null ? !type.IsValueType : type.IsAssignableFrom(value.GetType()));
		}
	}
}