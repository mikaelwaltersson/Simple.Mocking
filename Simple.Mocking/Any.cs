using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.Mocking.Syntax;

namespace Simple.Mocking
{
	public static class Any<T>
	{
		public static readonly AnyValueConstraint<T> Value = new AnyValueConstraint<T>();
	}
}
