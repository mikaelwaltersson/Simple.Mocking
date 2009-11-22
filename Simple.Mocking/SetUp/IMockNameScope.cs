using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
	interface IMockNameScope
	{
		bool Register(string name);
	}
}