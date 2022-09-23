using System;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class InvocationFactoryTests
	{
		[Test]
		public void CantInvokeGetForMethodWithNullArguments()
		{
			try
			{
				InvocationFactory.GetForMethod(null!);
			}
			catch (ArgumentNullException)
			{				
			}
		}

		[Test]
		public void CantInvokeGetFoMethodTextRepresentationWithNullArguments()
		{
			try
			{
				InvocationFactory.GetForMethodTextRepresentation(null!, "System.String ToString()");
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				InvocationFactory.GetForMethodTextRepresentation(typeof(object), null!);
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void CantInvokeGetTextRepresentationForMethodWithNullArguments()
		{
			try
			{
				InvocationFactory.GetTextRepresentationForMethod(null!);
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void LookupNonExistentMethodThrowsException()
		{
			try
			{
				InvocationFactory.GetForMethodTextRepresentation(typeof(object), "Boolean Equals(System.String)");
			}
			catch (MissingMemberException)
			{				
			}
		}
	}
}
