﻿using System;
using System.Runtime.Serialization;

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

		internal ExpectationsException(IInvocationHistory invocationHistory, string format, params object[] args)
			: this(FormatMessage(invocationHistory, format, args))
		{
		}

		protected ExpectationsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		static string FormatMessage(object details, string format, object[] args) =>
			string.Concat(string.Format(format, args), Environment.NewLine, Environment.NewLine, details);
	}
}
