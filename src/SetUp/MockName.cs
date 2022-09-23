using System;

namespace Simple.Mocking.SetUp
{
    static class MockName<T>
	{
		public static string GetUniqueInScope(IMockNameScope scope)
		{
			var nameBase = GetDefaultName();
			var suffix = string.Empty;

			for (var nextSuffix = 2; ; nextSuffix++)
			{
				var name = nameBase + suffix;

				if (scope.Register(name))
					return name;

				suffix = nextSuffix.ToString();
			}
		}

		static string GetDefaultName()
		{
			var type = typeof(T);
			var name = type.Name;

			if (type.IsInterface && GetFirstWord(name) == "I")
				name = name.Substring(1);

            if (type.IsGenericType)
            {
                var separatorIndex = name.LastIndexOf("`", StringComparison.InvariantCulture);
                name = name.Substring(0, separatorIndex);
            }

		    var firstWord = GetFirstWord(name);

			return firstWord.ToLower() + name.Substring(firstWord.Length);
		}

		static string GetFirstWord(string name)
		{
			var i = 1;

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