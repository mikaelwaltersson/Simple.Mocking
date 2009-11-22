using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
	interface IParameterValueConstraint
	{
		bool Matches(object value);
	}
}