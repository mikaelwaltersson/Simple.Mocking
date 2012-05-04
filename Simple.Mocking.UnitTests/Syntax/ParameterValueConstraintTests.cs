using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.Syntax;


namespace Simple.Mocking.UnitTests.Syntax
{
	[TestFixture]
	public class ParameterValueConstraintTests
	{
		[Test]
		public void ToStringDescribesConstraint()
		{
			Assert.AreEqual(new AnyValueConstraint<object>().ToString(), "Any<Object>.Value");
			Assert.AreEqual(new AnyValueConstraint<string>().ToString(), "Any<String>.Value");
			Assert.AreEqual(new AnyValueConstraint<int>().ToString(), "Any<Int32>.Value");

			Assert.AreEqual(new MatchingPredicateValueConstraint<int>(p => (p%2) == 0).ToString(), "Any<Int32>.Value.Matching(p => ((p % 2) == 0))");
		}

		[Test]
		public void AnyValueConstraintMatchesAnyValueOfType()
		{
			var constraint = new AnyValueConstraint<byte>() as IParameterValueConstraint;

			for (int i = byte.MinValue; i <= byte.MaxValue; i++)
				Assert.IsTrue(constraint.Matches((byte)i));
			
			Assert.IsFalse(constraint.Matches(new object()));
			Assert.IsFalse(constraint.Matches(string.Empty));
		}

		[Test]
		public void MatchingPredicateValueConstraint()
		{
			var constraint = new MatchingPredicateValueConstraint<int>(p => p > 0 && p < 5) as IParameterValueConstraint;

			Assert.IsFalse(constraint.Matches(0));
			Assert.IsTrue(constraint.Matches(1));
			Assert.IsTrue(constraint.Matches(2));
			Assert.IsTrue(constraint.Matches(3));
			Assert.IsTrue(constraint.Matches(4));
			Assert.IsFalse(constraint.Matches(5));
		}

		[Test]
		public void CantCreateMatchingPredicateValueConstraintWithNullPredicate()
		{
			try
			{
				new MatchingPredicateValueConstraint<object>(null);	
			}
			catch (ArgumentNullException)
			{				
			}
		}

		[Test]
		public void AnyValueConstraintForReferenceTypesMatchesNull()
		{
			Assert.IsTrue(((IParameterValueConstraint)Any<string>.Value).Matches(null));
		}

		[Test]
		public void AnyValueConstraintForValueTypesDoesNotMatchNull()
		{
			Assert.IsFalse(((IParameterValueConstraint)Any<byte>.Value).Matches(null));				
		}

		[Test]
		public void ImplicitConversionToValueThrowsException()
		{
		    Assert.Throws<InvalidOperationException>(() => Math.Max(0, new AnyValueConstraint<int>()));
		}

	}
}
