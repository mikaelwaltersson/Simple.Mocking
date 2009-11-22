using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Simple.Mocking.SetUp;

namespace Simple.Mocking
{
	[Serializable]
	public class ExpectationsException : Exception
	{
		public ExpectationsException(string message)
			: base(message)
		{
		}

		internal ExpectationsException(IExpectationScope expectationScope, string format, params object[] args)
			: this(FormatMessage(expectationScope, format, args))
		{
		}

		protected ExpectationsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}



		static string FormatMessage(IExpectationScope expectationScope, string format, object[] args)
		{
			return string.Format(format, args) + Environment.NewLine + Environment.NewLine + expectationScope;
		}

	}
}
