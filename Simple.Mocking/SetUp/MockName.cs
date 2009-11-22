using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
	static class MockName<T>
	{
		public static string GetUniqueInScope(IMockNameScope scope)
		{
			string nameBase = GetDefaultName();
			string suffix = string.Empty;

			for (int nextSuffix = 2; ; nextSuffix++)
			{
				string name = nameBase + suffix;

				if (scope.Register(name))
					return name;

				suffix = nextSuffix.ToString();
			}
		}

		static string GetDefaultName()
		{
			var type = typeof(T);
			string name = type.Name;

			if (type.IsInterface && GetFirstWord(name) == "I")
				name = name.Substring(1);

			string firstWord = GetFirstWord(name);

			return firstWord.ToLower() + name.Substring(firstWord.Length);
		}

		static string GetFirstWord(string name)
		{
			int i = 1;

			for (; i < name.Length; i++)
			{
				if (char.IsUpper(name[i]))
					break;
			}

			for (; i < name.Length; i++)
			{
				if (char.IsLower(name[i]))
					return name.Substring(0, Math.Max(0, i - 1));
			}

			return name;
		}
	}
}