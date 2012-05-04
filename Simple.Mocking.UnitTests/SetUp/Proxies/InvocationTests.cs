using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class InvocationTests
	{		
		TestInvocationInterceptor invocationInterceptor;
		IProxy target;

		[SetUp]
		public void SetUp()
		{
			invocationInterceptor = new TestInvocationInterceptor();
			target = new Proxy { InvocationInterceptor = invocationInterceptor, ProxiedType = null };			
		}

		[Test]
		public void CantSetReturnValueToNonAssignableValue()
		{
			var invocation = new Invocation(target, typeof(IMyInterface).GetMethod("Method"), null, new object[0], default(int), 0);

		    Assert.Throws<InvalidOperationException>(() => invocation.ReturnValue = null);
		}

		[Test]
		public void CantSetReturnValueForMethodWithoutReturnValue()
		{
			var invocation = new Invocation(target, typeof(IMyInterface).GetMethod("MethodWithoutReturnValue"), null, new object[2], null, 0);

            Assert.Throws<InvalidOperationException>(() => invocation.ReturnValue = 0);
		}

		[Test]
		public void CantSetParameterValueToNonAssignableValue()
		{
			var invocation = new Invocation(target, typeof(IMyInterface).GetMethod("MethodWithOutAndRefValues"), null, new object[2], null, 0);

            Assert.Throws<InvalidOperationException>(() => invocation.ParameterValues[0] = new object());
		}

		[Test]
		public void CantSetParameterValueForNonOutOrRefParameter()
		{
			var invocation = new Invocation(target, typeof(IMyInterface).GetMethod("MethodWithInputValue"), null, new object[1], null, 0);

            Assert.Throws<InvalidOperationException>(() => invocation.ParameterValues[0] = 0);
		}


		[Test]
		public void SettingOutAndRefParameters()
		{
			var method = typeof(IMyInterface).GetMethod("MethodWithOutAndRefValues");

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(42, (int)invocation.ParameterValues[0]);
					invocation.ParameterValues[0] = 43;
					invocation.ParameterValues[1] = 44;					
				};

			var parameters = new object[] { 42, null };
			
			Invocation.HandleInvocation(target, InvocationFactory.GetForMethod(method), null, parameters, null);

			Assert.AreEqual(43, parameters[0]);
			Assert.AreEqual(44, parameters[1]);
		}

		[Test]
		public void ParameterAndGenericArgumentsListsHasRestrictedAccess()
		{
			var item = new object();
			var type = typeof(object);

			var invocation = 
				new Invocation(
					target, 
					typeof(IMyInterface).GetMethod("MethodWithGenericArguments"),
					new List<Type> { type }, 
					new List<object> { item },
					null,
                    0);

			var parameterValues = invocation.ParameterValues;
			var genericArguments = invocation.GenericArguments;

			AssertIsNotSupported(() => parameterValues.Add(item));
			AssertIsNotSupported(() => genericArguments.Add(type));

			AssertIsNotSupported(() => parameterValues.Remove(item));
			AssertIsNotSupported(() => genericArguments.Remove(type));

			AssertIsNotSupported(() => parameterValues.RemoveAt(0));
			AssertIsNotSupported(() => genericArguments.RemoveAt(0));

			AssertIsNotSupported(() => parameterValues.Insert(0, item));
			AssertIsNotSupported(() => genericArguments.Insert(0, type));

			AssertIsNotSupported(parameterValues.Clear);
			AssertIsNotSupported(parameterValues.Clear);


			Assert.AreEqual(1, parameterValues.Count);
			Assert.AreEqual(1, genericArguments.Count);

			Assert.IsTrue(parameterValues.Contains(item));
			Assert.IsTrue(genericArguments.Contains(type));

			Assert.AreEqual(0, parameterValues.IndexOf(item));
			Assert.AreEqual(0, genericArguments.IndexOf(type));

			Assert.AreEqual(item, parameterValues[0]);
			Assert.AreEqual(type, genericArguments[0]);

			parameterValues[0] = item;
			AssertIsNotSupported(() => genericArguments[0] = type);

			var arrayCopy = new object[1];
			parameterValues.CopyTo(arrayCopy, 0);
			Assert.AreEqual(item, arrayCopy[0]);

			var arrayCopy2 = new Type[1];
			genericArguments.CopyTo(arrayCopy2, 0);
			Assert.AreEqual(type, arrayCopy2[0]);

			var enumerator = ((System.Collections.IEnumerable)parameterValues).GetEnumerator();			
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(item, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());

			var enumerator2 = ((System.Collections.IEnumerable)genericArguments).GetEnumerator();
			Assert.IsTrue(enumerator2.MoveNext());
			Assert.AreEqual(type, enumerator2.Current);
			Assert.IsFalse(enumerator2.MoveNext());

			Assert.IsFalse(parameterValues.IsReadOnly);
			Assert.IsFalse(genericArguments.IsReadOnly);
		}


		void AssertIsNotSupported(Action action)
		{
            Assert.Throws<NotSupportedException>(() => action());
		}



		class Proxy : IProxy
		{
			public Type ProxiedType { get; set; }
			public object BaseObject { get { return this; } }
			public IInvocationInterceptor InvocationInterceptor { get; set; }
		}

		interface IMyInterface
		{
			int Method();
			void MethodWithGenericArguments<T>(ref object value);
			void MethodWithOutAndRefValues(out int outValue, ref int refValue);
			void MethodWithInputValue(int value);			
			void MethodWithoutReturnValue();
		}
	}
}