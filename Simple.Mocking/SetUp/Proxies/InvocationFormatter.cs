using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	static class InvocationFormatter
	{
		public static string Format(object target, MethodInfo method, IList<object> parameterValues)
		{
			var genericArguments = GetGenericArguments(method);

			return Format(target, method, genericArguments, parameterValues);
		}

	    public static string Format(object target, MethodInfo method, IList<Type> genericArguments, IList<object> parameterValues)
		{
            var unwrappedTarget = InvocationTarget.UnwrapDelegateTargetAndProxyBaseObject(target);
         
            if (InvocationTarget.IsDelegate(target))
                return FormatDelegate(unwrappedTarget, parameterValues);

            if (method == null)
                return FormatWildcardInvocation(unwrappedTarget);
    
            var property = method.GetDeclaringProperty();
            if (property != null)
                return FormatProperty(unwrappedTarget, method, property, parameterValues);

            var @event = method.GetDeclaringEvent();
            if (@event != null)
                return FormatEvent(unwrappedTarget, method, @event, parameterValues);

            return FormatMethod(unwrappedTarget, genericArguments, method, parameterValues);
        }





        static Type[] GetGenericArguments(MethodInfo method)
        {
            if (method == null || !method.IsGenericMethod)
                return null;

            return method.GetGenericArguments();
        }


		static string FormatParameterValue(object value)
		{
			if (value == null)
				return "null";

			if (value is string)
				return FormatStringParameterValue((string)value);

			if (value is char)
				return FormatCharParameterValue((char)value);
			
			return FormatString("{0}", value);
		}

		static string FormatStringParameterValue(string value)
		{
			return value.
				Select(c => EscapeChar(c)).
				Aggregate(
					new StringBuilder("\""), 
					(result, c) => result.Append(c)).
				Append("\"").
				ToString();
		}

		static string FormatCharParameterValue(char value)
		{
			return "'" + EscapeChar(value) + "'";
		}

		static string EscapeChar(char c)
		{
			const string charsToEscape = "'\"\\\0\a\b\f\n\r\t\v";			
			const string escapePrefix = "\\";
			const string escapedChars = "'\"\\0abfnrtv";


			var escapedCharsIndex = charsToEscape.IndexOf(c);
			
			if (escapedCharsIndex >= 0)
				return escapePrefix + escapedChars[escapedCharsIndex];

			return c.ToString();
		}

        static string FormatMethod(object unwrappedTarget, IList<Type> genericArguments, MethodInfo method, IList<object> parameterValues)
        {
            return FormatString("{0}.{1}{2}({3})", unwrappedTarget, method.Name, FormatGenericArguments(genericArguments), FormatParameters(parameterValues)); 
        }
       
        static string FormatWildcardInvocation(object unwrappedTarget)
        {
            return FormatString("{0}.*", unwrappedTarget);
        }

		static string FormatProperty(object unwrappedTarget, MethodInfo method, PropertyInfo property, IList<object> parameterValues)
		{
			var valueAssignmentText = string.Empty;

			if (property.GetSetMethod() == method)
			{
                valueAssignmentText = FormatString(" = {0}", FormatParameterValue(parameterValues.Last()));
			    parameterValues = parameterValues.Take(parameterValues.Count - 1).ToArray();
			}

		    var propertyText =
		        (parameterValues.Count > 0)
		            ? FormatString("[{0}]", FormatParameters(parameterValues))
		            : FormatString(".{0}", property.Name);

			return FormatString("{0}{1}{2}", unwrappedTarget, propertyText, valueAssignmentText);
		}

		static string FormatEvent(
            object unwrappedTarget, MethodInfo method, EventInfo @event, IList<object> parameterValues)
		{
			var operationText = (@event.GetAddMethod() == method ? "+" : "-");

            return FormatString("{0}.{1} {2}= {3}", unwrappedTarget, @event.Name, operationText, FormatParameters(parameterValues));
		}

        static string FormatDelegate(object unwrappedTarget, IList<object> parameterValues)
        {
            return FormatString("{0}({1})", unwrappedTarget, FormatParameters(parameterValues));
        }

        static string FormatParameters(IList<object> parameterValues)
        {
            if (parameterValues == null)
                return "*";

            return FormatList(parameterValues, FormatParameterValue);
        }

        static string FormatGenericArguments(IList<Type> genericArguments)
        {
            var genericArgumentsText = FormatList(genericArguments, type => type.Name);

            if (genericArgumentsText.Length > 0)
                genericArgumentsText = "<" + genericArgumentsText + ">";

            return genericArgumentsText;
        }



		static string FormatList<T>(IList<T> list, Func<T, string> converter)
		{
			if (list == null || list.Count == 0)
				return string.Empty;

            return string.Join(", ", list.Select(converter).ToArray());
		}


        static string FormatString(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
	}
}