using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ByRefTypeTests
	{
		[Test]
		public void GetRealTypeForByRefType()
		{
			Assert.AreEqual(typeof(int), typeof(int).MakeByRefType().GetRealTypeForByRefType());
		}


		[Test]
		public void CantInvokeGetRealTypeForByRefTypeForNonByRefTypes()
		{
		    Assert.Throws<InvalidOperationException>(() => typeof(int).GetRealTypeForByRefType());
		}

	}
}
