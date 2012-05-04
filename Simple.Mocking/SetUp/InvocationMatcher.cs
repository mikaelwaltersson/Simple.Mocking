using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Simple.Mocking.SetUp.Proxies;
using Simple.Mocking.Syntax;

namespace Simple.Mocking.SetUp
{
	class InvocationMatcher : IInvocationMatcher
	{
		object target;
		MethodInfo method;
		IList<object> parameterValueConstraints;

		public InvocationMatcher(object target, MethodInfo method, IList<object> parameterValueConstraints)
		{
            if (InvocationTarget.IsDelegate(target) && method != null)
                throw new ArgumentException("Can not specify delegate target and method", "method");

			this.target = target;
			this.method = method;
			this.parameterValueConstraints = parameterValueConstraints;
		}

		public object Target
		{
			get { return target; }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public IList<object> ParameterValueConstraints
		{
			get { return parameterValueConstraints; }
		}

		public bool Matches(IInvocation invocation)
		{
			return 
				MatchesTarget(invocation.Target) &&
				MatchesMethod(invocation.Method, invocation.GenericArguments) &&
				MatchesParameters(invocation.ParameterValues);
		}

		bool MatchesTarget(IProxy invocationTarget)
		{
			return ReferenceEquals(invocationTarget, InvocationTarget.UnwrapDelegateTarget(target));
		}

		bool MatchesMethod(MethodInfo invocationMethod, IEnumerable<Type> genericArguments)
		{
            if (method == null)
                return (invocationMethod.DeclaringType != typeof(object));

			if (genericArguments != null)
				invocationMethod = invocationMethod.MakeGenericMethod(genericArguments.ToArray());

			return method.Equals(invocationMethod);
		}

		bool MatchesParameters(IList<object> invocationParameterValues)
		{
            if (parameterValueConstraints == null)
                return true;

			if (invocationParameterValues.Count != parameterValueConstraints.Count)
				return false;

			for (var i = 0; i < parameterValueConstraints.Count; i++)
			{
				var constraint = parameterValueConstraints[i];
				var value = invocationParameterValues[i];
				
                bool isMatch;

				if (constraint is IParameterValueConstraint)
					isMatch = ((IParameterValueConstraint)constraint).Matches(value);
				else if (constraint == null)
					isMatch = (value == null);
				else
					isMatch = constraint.Equals(value);

				if (!isMatch)
					return false;
			}

			return true;
		}



		public override string ToString()
		{
			return InvocationFormatter.Format(target, method, parameterValueConstraints);			
		}

		static InvocationMatcher CreateInvocationMatcher(object target, MethodInfo method, IEnumerable<object> parameters)
		{
			if (method.IsGenericMethod)
				method = StripParameterValueConstraintsFromGenericArguments(method);

			return new InvocationMatcher(target, method, parameters.ToList());
		}

		static MethodInfo StripParameterValueConstraintsFromGenericArguments(MethodInfo method)
		{
			var genericArguments = 
				from type in method.GetGenericArguments()
				select ParameterValueConstraintTypeToIntendedType(type);

			return method.GetGenericMethodDefinition().MakeGenericMethod(genericArguments.ToArray());
		}

		static Type ParameterValueConstraintTypeToIntendedType(Type type)
		{
			Type[] genericArguments;

			if (type.IsGenericType && 
				(genericArguments = type.GetGenericArguments()).Length == 1 &&
				typeof(ParameterValueConstraint<>).MakeGenericType(genericArguments).IsAssignableFrom(type))
			{				
				type = type.GetGenericArguments()[0];
			}

			return type;
		}

		public static InvocationMatcher ForMethodCall(LambdaExpression methodCallExpression)
		{
			if (IsDelegateMethodCall(methodCallExpression))
				return CreateForDelegateMethodCall(methodCallExpression);

			if (IsMethodCall(methodCallExpression))
				return CreateForMethodCall(methodCallExpression);

			throw new ArgumentException(
				"Expected method call expression: '() => myObject.MyMethod()' or '() => myDelegateHandler()'",
				"methodCallExpression");
		}

		static bool IsMethodCall(LambdaExpression methodCallExpression)
		{
			var body = methodCallExpression.Body as MethodCallExpression;

			return (body != null && body.Method.GetDeclaringProperty() == null && body.Method.GetDeclaringEvent() == null);
		}

		static InvocationMatcher CreateForMethodCall(LambdaExpression methodCallExpression)
		{
			var body = (MethodCallExpression)methodCallExpression.Body;

			var target = ResolveObjectFromExpression(body.Object);
			var parameters = ParseParameters(body.Arguments, body.Method);

			return CreateInvocationMatcher(target, body.Method, parameters);
		}

		static bool IsDelegateMethodCall(LambdaExpression methodCallExpression)
		{
			var body = methodCallExpression.Body as InvocationExpression;

			return (body != null && ResolveObjectFromExpression(body.Expression) is Delegate);
		}

		static InvocationMatcher CreateForDelegateMethodCall(LambdaExpression delegateCallExpression)
		{
			var body = (InvocationExpression)delegateCallExpression.Body;
			
			var target = (Delegate)ResolveObjectFromExpression(body.Expression);
			var parameters = ParseParameters(body.Arguments, target.Method);

			return new InvocationMatcher(target, null, parameters.ToList());
		}


		public static InvocationMatcher ForPropertyGet(LambdaExpression propertyExpression)
		{
			return ForPropertyCall(propertyExpression, property => property.GetGetMethod());
		}

		public static InvocationMatcher ForPropertySet(LambdaExpression propertyExpression, object value)
		{
			return ForPropertyCall(propertyExpression, property => property.GetSetMethod(), value);
		}



		static bool IsPropertyExpression(LambdaExpression propertyExpression)
		{
			var body = propertyExpression.Body as MemberExpression;

			return (body != null && body.Member is PropertyInfo);
		}

		static bool IsIndexedPropertyExpression(LambdaExpression propertyExpression)
		{
			var body = propertyExpression.Body as MethodCallExpression;

			return (body != null && body.Method.GetDeclaringProperty() != null);
		}

		

		static InvocationMatcher ForPropertyCall(LambdaExpression propertyExpression, Func<PropertyInfo, MethodInfo> methodSelector, params object[] parameters)
		{
			if (IsIndexedPropertyExpression(propertyExpression))
				return CreateForIndexedPropertyCall(propertyExpression, methodSelector, parameters);

			if (IsPropertyExpression(propertyExpression))
				return CreateForPropertyCall(propertyExpression, methodSelector, parameters);

			throw new ArgumentException(
				"Expected property expression: '() => myObject.Property' or '() => myObject[i]'",
				"propertyExpression");
		}

		static InvocationMatcher CreateForPropertyCall(LambdaExpression propertyExpression, Func<PropertyInfo, MethodInfo> methodSelector, object[] parameters)
		{
			var body = (MemberExpression)propertyExpression.Body;
			var property = (PropertyInfo)body.Member;

			var target = ResolveObjectFromExpression(body.Expression);
			var method = methodSelector(property);

			return CreateInvocationMatcher(target, method, parameters);
		}

		static InvocationMatcher CreateForIndexedPropertyCall(LambdaExpression propertyExpression, Func<PropertyInfo, MethodInfo> methodSelector, object[] valueParameters)
		{
			var body = (MethodCallExpression)propertyExpression.Body;

			var target = ResolveObjectFromExpression(body.Object);
			var method = methodSelector(body.Method.GetDeclaringProperty());
			var parameters = ParseParameters(body.Arguments, body.Method).Concat(valueParameters);

			return CreateInvocationMatcher(target, method, parameters);
		}

		public static InvocationMatcher ForEventAdd<T>(T target, string eventName, object handler)
		{
			return CreateForEventAddOrRemove(target, GetEvent<T>(eventName).GetAddMethod(), handler);
		}

		public static InvocationMatcher ForEventRemove<T>(T target, string eventName, object handler)
		{
			return CreateForEventAddOrRemove(target, GetEvent<T>(eventName).GetRemoveMethod(), handler);
		}

		static EventInfo GetEvent<T>(string eventName)
		{
			var eventMember = typeof(T).GetEvent(eventName);

			if (eventMember == null)
				throw new ArgumentException(string.Format("Event '{0}' is not defined in type '{1}'", eventName, typeof(T)), "eventName");

			return eventMember;
		}

		static InvocationMatcher CreateForEventAddOrRemove(object target, MethodInfo method, object handler)
		{
			return CreateInvocationMatcher(target, method, new[] { handler });
		}


		static IEnumerable<object> ParseParameters(IEnumerable<Expression> arguments, MethodInfo methodInfo)
		{
			var methodParameters = methodInfo.GetParameters();

			return arguments.Select((argument, i) => ParseParameterConstraint(argument, methodParameters[i]));
		}

		static object ResolveObjectFromExpression(Expression expression)
		{
			return Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object))).Compile()();
		}

		static object ParseParameterConstraint(Expression argumentExpression, ParameterInfo parameter)
		{
			return ResolveObjectFromExpression(GetReducedParameterValueConstraintExpression(argumentExpression, parameter));
		}

		static Expression GetReducedParameterValueConstraintExpression(Expression argumentExpression, ParameterInfo parameter)
		{
			var reducedExpression = argumentExpression;

			while (reducedExpression.NodeType == ExpressionType.Convert && reducedExpression is UnaryExpression)			
				reducedExpression = ((UnaryExpression)reducedExpression).Operand;			

			if (reducedExpression.NodeType == ExpressionType.MemberAccess && reducedExpression is MemberExpression)
			{
				var memberExpression = (MemberExpression)reducedExpression;

				if (IsAsRefOrOutExpression(memberExpression))
				{
					AssertParameterTypeIsByRef(memberExpression, parameter);
					return memberExpression.Expression;
				}
				
				if (IsAsInterfaceExpression(memberExpression))
				{
					AssertGenericArgumentIsInterface(memberExpression);
					return memberExpression.Expression;
				}
			}

			if (typeof(IParameterValueConstraint).IsAssignableFrom(reducedExpression.Type))
				return reducedExpression;

			return argumentExpression;
		}

		static bool IsAsRefOrOutExpression(MemberExpression memberExpression)
		{
			return IsFieldInGenericTypeDefinition(memberExpression.Member, typeof(ParameterValueConstraint<>), "AsRefOrOut");
		}

		static bool IsAsInterfaceExpression(MemberExpression memberExpression)
		{
			return IsFieldInGenericTypeDefinition(memberExpression.Member, typeof(ParameterValueConstraint<>), "AsInterface");
		}

		static bool IsFieldInGenericTypeDefinition(MemberInfo member, Type genericType, string fieldName)
		{
			var declaringType = member.DeclaringType;

			return (
				declaringType.IsGenericType && 
				declaringType.GetGenericTypeDefinition() == genericType && 
				member is FieldInfo && 
				member.Name == fieldName);
		}

		static void AssertParameterTypeIsByRef(MemberExpression memberExpression, ParameterInfo parameter)
		{
			if (!parameter.ParameterType.IsByRef)
				throw new ArgumentException(
					string.Format("Cant set '{0}' as an value constraint for non ref/out parameter {1}", memberExpression, parameter.Name));
		}

		static void AssertGenericArgumentIsInterface(MemberExpression memberExpression)
		{
			var genericArgument = memberExpression.Member.DeclaringType.GetGenericArguments()[0];

			if (!genericArgument.IsInterface)
				throw new ArgumentException(string.Format("Cant set '{0}' as an value constraint for non interface type {1}", memberExpression, genericArgument));
		}

		public static InvocationMatcher ForAnyInvocationOn(object target)
		{
			return new InvocationMatcher(target, null, null);
		}
	}
}