using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	static class InvocationFormatter
	{
		public static string Format(object target, MethodInfo method, IList<object> parameterValues)
		{
			var genericArguments = (method != null && method.IsGenericMethod ? method.GetGenericArguments() : null);

			return Format(target, method, genericArguments, parameterValues ?? new object[0]);
		}

		public static string Format(object target, MethodInfo method, IList<Type> genericArguments, IList<object> parameterValues)
		{
			string targetText = FormatTarget(target);

			string methodNameText = string.Empty;
			string genericArgumentsText = FormatList(genericArguments, "<", ">", item => item.Name);

			var formattedParameterValues = Array.ConvertAll(parameterValues.ToArray(), value => FormatParameterValue(value));
			
			return Format(target, targetText, method, methodNameText, genericArgumentsText, formattedParameterValues);
		}

		static string Format(
			object target, string targetText,
			MethodInfo method, string methodNameText, string genericArgumentsText, 
			object[] formattedParameterValues)
		{
			if (method != null)
			{
				var property = method.GetDeclaringProperty();
				if (property != null)
					return FormatProperty(targetText, genericArgumentsText, method, property, formattedParameterValues);

				var @event = method.GetDeclaringEvent();
				if (@event != null)
					return FormatEvent(targetText, method, @event, formattedParameterValues);

				methodNameText = method.Name;
			}
			
			string parametersValuesText = FormatList(formattedParameterValues, "(", ")");

			if (target is Delegate)
				return ((Delegate)target).Target + parametersValuesText;

			return targetText + "." + methodNameText + genericArgumentsText + parametersValuesText;
		}

		static object FormatParameterValue(object value)
		{
			if (value == null)
				return "null";

			if (value is string)
				return FormatStringParameterValue((string)value);

			if (value is char)
				return FormatCharParameterValue((char)value);
			
			return value;
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
			const string CharsToEscape = "'\"\\\0\a\b\f\n\r\t\v";
			
			const string EscapePrefix = "\\";
			const string EscapedChars = "'\"\\0abfnrtv";


			int escapedCharsIndex = CharsToEscape.IndexOf(c);
			
			if (escapedCharsIndex >= 0)
				return EscapePrefix + EscapedChars[escapedCharsIndex];

			return c.ToString();
		}

		static string FormatProperty(
			string targetText, string genericArgumentsText, 
			MethodInfo method, PropertyInfo property, object[] formattedParameterValues)
		{
			string propertyText = "." + property.Name;
			string valueText = string.Empty;

			if (property.GetSetMethod() == method)
			{
				valueText = string.Format(" = {0}", formattedParameterValues[formattedParameterValues.Length - 1]);
				Array.Resize(ref formattedParameterValues, formattedParameterValues.Length - 1);
			}

			if (formattedParameterValues.Length > 0)
				propertyText = FormatList(formattedParameterValues, "[", "]");

			return targetText + genericArgumentsText + propertyText + valueText;
		}


		static string FormatEvent(
			string targetText, MethodInfo method, EventInfo @event, object[] formattedParameterValues)
		{
			string operationText = (@event.GetAddMethod() == method ? "+" : "-");

			return string.Format("{0}.{1} {2}= {3}", targetText, @event.Name, operationText, formattedParameterValues[0]);
		}

		static string FormatList<T>(IList<T> list, string prefix, string suffix)
		{
			return FormatList(list, prefix, suffix, item => Convert.ToString(item));
		}
		static string FormatList<T>(IList<T> list, string prefix, string suffix, Func<T, string> converter)
		{
			if (list == null || list.Count == 0)
				return string.Empty;

			var itemsTextRepresentation = list.Select(converter).ToArray();

			return (prefix + string.Join(", ", itemsTextRepresentation) + suffix);
		}

		public static string FormatTarget(object target)
		{
			if (target is IProxy)
				target = ((IProxy)target).BaseObject;

			return Convert.ToString(target);
		}
	}
}