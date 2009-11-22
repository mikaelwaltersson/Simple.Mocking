using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
	[TestFixture]
	public class MockTests
	{
		[Test]
		public void InvokingUnexpectedActionThrowsException()
		{
			var myObject = Mock.Interface<IMyObject>();

			try
			{
				myObject.MyEmptyMethod();

				Assert.Fail();
			}
			catch (ExpectationsException)
			{
			}
		}
	}
}
