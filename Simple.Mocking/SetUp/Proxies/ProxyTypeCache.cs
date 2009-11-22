using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	class ProxyTypeCache
	{
		IDictionary<Type, Entry> cache;

		public ProxyTypeCache()
		{
			cache = new Dictionary<Type, Entry>();
		}



		public Type GetProxyType(Type type, Func<Type, Type> createTypeDelegate)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (createTypeDelegate == null)
				throw new ArgumentNullException("createTypeDelegate");

			Entry entry;

			if (!cache.TryGetValue(type, out entry))
			{
				entry = new Entry();
				cache.Add(type, entry);

				try
				{
					entry.Type = createTypeDelegate(type);
				}
				catch (Exception ex)
				{
					entry.CreateTypeException = ex;
				}
			}

			return entry.Type;
		}

		class Entry
		{
			Type type;
			Exception createTypeException;

			public Type Type
			{
				set { this.type = value; }
				get
				{
					if (type == null)
						throw new InvalidOperationException("Mock type creation failed", createTypeException);

					return type;
				}
			}

			public Exception CreateTypeException
			{
				set { createTypeException = value; }
			}
		}

	}
}