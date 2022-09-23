using System.Linq;
using System.Reflection;

namespace Simple.Mocking.SetUp
{
	static class Method
	{
		public static PropertyInfo? GetDeclaringProperty(this MethodInfo method) =>
			method.DeclaringType!.GetProperties().FirstOrDefault(
				property => (property.GetSetMethod() == method || property.GetGetMethod() == method));

		public static EventInfo? GetDeclaringEvent(this MethodInfo method) =>
			method.DeclaringType!.GetEvents().FirstOrDefault(
				@event => (@event.GetAddMethod() == method || @event.GetRemoveMethod() == method));		
	}
}