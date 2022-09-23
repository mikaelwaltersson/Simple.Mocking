using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.UnitTests
{
	[TestFixture]
	public class ExpectationsExceptionTests
	{
		[Test]
		public void IsSerializable()
		{
			var formatter = new BinaryFormatter();

			using (var stream = new MemoryStream())
			{
#pragma warning disable SYSLIB0011
				formatter.Serialize(stream, new ExpectationsException("error"));
				stream.Position = 0;

				Assert.AreEqual("error", ((ExpectationsException)formatter.Deserialize(stream)).Message);
#pragma warning restore SYSLIB0011
			}			
		}
	}
}