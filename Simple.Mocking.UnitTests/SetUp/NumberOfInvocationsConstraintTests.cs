using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.UnitTests.SetUp
{
	[TestFixture]
	public class NumberOfInvocationsConstraintTests
	{
		[Test]
		public void Matches()
		{
			Assert.IsTrue(new NumberOfInvocationsConstraint(null, null).Matches(0));
			Assert.IsTrue(new NumberOfInvocationsConstraint(null, null).Matches(int.MaxValue));

			Assert.IsTrue(new NumberOfInvocationsConstraint(10, null).Matches(int.MaxValue));
			Assert.IsTrue(new NumberOfInvocationsConstraint(10, null).Matches(10));
			Assert.IsFalse(new NumberOfInvocationsConstraint(10, null).Matches(9));

			Assert.IsTrue(new NumberOfInvocationsConstraint(null, 15).Matches(0));
			Assert.IsTrue(new NumberOfInvocationsConstraint(null, 15).Matches(15));
			Assert.IsFalse(new NumberOfInvocationsConstraint(null, 15).Matches(16));

			Assert.IsFalse(new NumberOfInvocationsConstraint(10, 15).Matches(9));
			Assert.IsTrue(new NumberOfInvocationsConstraint(10, 15).Matches(10));
			Assert.IsTrue(new NumberOfInvocationsConstraint(10, 15).Matches(15));
			Assert.IsFalse(new NumberOfInvocationsConstraint(10, 15).Matches(16));
		}

		[Test]
		public void CanInvoke()
		{
			Assert.IsTrue(new NumberOfInvocationsConstraint(null, null).CanInvoke(int.MaxValue - 1));

			Assert.IsTrue(new NumberOfInvocationsConstraint(10, null).CanInvoke(0));
			Assert.IsTrue(new NumberOfInvocationsConstraint(10, null).CanInvoke(int.MaxValue - 1));

			Assert.IsTrue(new NumberOfInvocationsConstraint(null, 15).CanInvoke(int.MinValue));
			Assert.IsTrue(new NumberOfInvocationsConstraint(null, 15).CanInvoke(14));
			Assert.IsFalse(new NumberOfInvocationsConstraint(null, 15).CanInvoke(15));

			Assert.IsTrue(new NumberOfInvocationsConstraint(10, 15).CanInvoke(0));
			Assert.IsTrue(new NumberOfInvocationsConstraint(10, 15).CanInvoke(14));
			Assert.IsFalse(new NumberOfInvocationsConstraint(10, 15).CanInvoke(15));
		}

		[Test]
		public void CanToString()
		{
			Assert.AreEqual("*", new NumberOfInvocationsConstraint(null, null).ToString());
			Assert.AreEqual("10", new NumberOfInvocationsConstraint(10, 10).ToString());

			Assert.AreEqual("*..15", new NumberOfInvocationsConstraint(null, 15).ToString());
			Assert.AreEqual("10..*", new NumberOfInvocationsConstraint(10, null).ToString());
			Assert.AreEqual("10..15", new NumberOfInvocationsConstraint(10, 15).ToString());			
		}

	}
}