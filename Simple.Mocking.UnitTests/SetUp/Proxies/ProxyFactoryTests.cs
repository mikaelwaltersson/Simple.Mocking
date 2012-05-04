using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;


namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
	[TestFixture]
	public class ProxyFactoryTests : ProxyFactoryTestsBase
	{	
		[Test]
		public void CantInvokeConstructorWithNullArgument()
		{
		    Assert.Throws<ArgumentNullException>(() => new ProxyFactory(null));
		}

		[Test]
		public void CantInvokeCreateProxyWithNullArgument()
		{
            Assert.Throws<ArgumentNullException>(() => proxyFactory.CreateInterfaceProxy<EventHandler>(string.Empty, null));
            Assert.Throws<ArgumentNullException>(() => proxyFactory.CreateInterfaceProxy<EventHandler>(null, invocationInterceptor));
            Assert.Throws<ArgumentNullException>(() => proxyFactory.CreateDelegateProxy<EventHandler>(new object(), null));
            Assert.Throws<ArgumentNullException>(() => proxyFactory.CreateDelegateProxy<EventHandler>(null, invocationInterceptor));
		}


		[Test]
		public void InterfaceProxyImplementsIProxy()
		{
			var baseObject = new object();
			var proxy = proxyFactory.CreateInterfaceProxy<IEmptyInterface>(baseObject, invocationInterceptor) as IProxy;

			Assert.IsNotNull(proxy);

			Assert.AreEqual(typeof(IEmptyInterface), proxy.ProxiedType);
			Assert.AreSame(baseObject, proxy.BaseObject);
			Assert.AreSame(invocationInterceptor, proxy.InvocationInterceptor);
		}


		[Test]
		public void CreatingTwoInterfaceProxiesWithSameInterfaceReusesType()
		{
			var proxy1 = proxyFactory.CreateInterfaceProxy<IEmptyInterface>(baseObject, invocationInterceptor);
			var proxy2 = proxyFactory.CreateInterfaceProxy<IEmptyInterface>(baseObject, invocationInterceptor);

			Assert.IsNotNull(proxy1);
			Assert.IsNotNull(proxy2);
			Assert.AreSame(proxy1.GetType(), proxy2.GetType());
		}

		[Test]
		public void CreatingInterfaceProxyFromClassFails()
		{
            Assert.Throws<ArgumentException>(() => proxyFactory.CreateInterfaceProxy<EmptyAbstractClass>(baseObject, invocationInterceptor));
		}

		[Test]
		public void CreateInterfaceProxyForSubInterface()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<ISubInterface>(baseObject, invocationInterceptor);

			Assert.IsNotNull(proxy);
			Assert.IsInstanceOf<ISubInterface>(proxy);
		}

		[Test]
		public void InvokeMethodOnInterface()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithMethod).GetMethod("Method"), invocation.Method);

					var genericArguments = invocation.GenericArguments;

					Assert.IsNull(genericArguments);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(0, parameterValues.Count);
				};


			proxy.Method();

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeGenericMethodOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithGenericMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithGenericMethod).GetMethod("Method"), invocation.Method);

					var genericArguments = invocation.GenericArguments;

					Assert.AreEqual(2, genericArguments.Count);
					Assert.AreEqual(typeof(string), genericArguments[0]);
					Assert.AreEqual(typeof(int), genericArguments[1]);


					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual("123", parameterValues[0]);

					invocation.ReturnValue = 123;
				};


			int returnValue = proxy.Method<string, int>("123");

			Assert.AreEqual(123, returnValue);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeGenericMethodWithInterfaceConstraintsOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithGenericMethodWithInterfaceTypeConstraints>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithGenericMethodWithInterfaceTypeConstraints).GetMethod("Method"), invocation.Method);

					var genericArguments = invocation.GenericArguments;

					Assert.AreEqual(2, genericArguments.Count);
					Assert.AreEqual(typeof(string), genericArguments[0]);
					Assert.AreEqual(typeof(ProxyFactoryTests), genericArguments[1]);


					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual("123", parameterValues[0]);

					invocation.ReturnValue = this;
				};


			var returnValue = proxy.Method<string, ProxyFactoryTests>("123");

			Assert.AreEqual(this, returnValue);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeGenericMethodWithTypeBaseTypeConstraintOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithGenericMethodWithBaseTypeConstraint>(baseObject, invocationInterceptor);

			var expectedInput = new DerivedFromBaseTypeConstraintClass();

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithGenericMethodWithBaseTypeConstraint).GetMethod("Method"), invocation.Method);

					var genericArguments = invocation.GenericArguments;

					Assert.AreEqual(2, genericArguments.Count);
					Assert.AreEqual(typeof(DerivedFromBaseTypeConstraintClass), genericArguments[0]);
					Assert.AreEqual(typeof(int), genericArguments[1]);


					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(expectedInput, parameterValues[0]);

					invocation.ReturnValue = 0;
				};


			var returnValue = proxy.Method<DerivedFromBaseTypeConstraintClass, int>(expectedInput);

			Assert.AreEqual(0, returnValue);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}


		[Test]
		public void InvokeMethodOnGenericInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IGenericInterfaceWithMethod<string, int>>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IGenericInterfaceWithMethod<string, int>).GetMethod("Method"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual("123", parameterValues[0]);

					invocation.ReturnValue = 123;
				};


			int returnValue = proxy.Method("123");

			Assert.AreEqual(123, returnValue);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeEventAdderOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithEvent>(baseObject, invocationInterceptor);

			var onEventHandler = new EventHandler(delegate { });

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithEvent).GetEvent("OnEvent").GetAddMethod(), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(onEventHandler, parameterValues[0]);
				};


			proxy.OnEvent += onEventHandler;

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeEventRemoverOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithEvent>(baseObject, invocationInterceptor);

			var onEventHandler = new EventHandler(delegate { });

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithEvent).GetEvent("OnEvent").GetRemoveMethod(), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(onEventHandler, parameterValues[0]);
				};


			proxy.OnEvent -= onEventHandler;

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeToStringDeclaredInObjectOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(object).GetMethod("ToString"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(0, parameterValues.Count);

					invocation.ReturnValue = string.Empty;
				};


			proxy.ToString();
			
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeEqualsDeclaredInObjectOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(object).GetMethod("Equals", new[]{ typeof(object) }), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreSame(proxy, parameterValues[0]);

					invocation.ReturnValue = true;
				};


			proxy.Equals(proxy);

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeGetHashCodeDeclaredInObjectOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(object).GetMethod("GetHashCode"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(0, parameterValues.Count);

					invocation.ReturnValue = 0;
				};
			
			proxy.GetHashCode();

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}


		[Test]
		public void DelegateProxyImplementsIProxy()
		{
			var baseObject = new object();
			var proxy = proxyFactory.CreateDelegateProxy<EventHandler>(baseObject, invocationInterceptor);
			
			Assert.IsNotNull(proxy);
			Assert.IsInstanceOf(typeof(IProxy), proxy.Target);

			var proxyTarget = (IProxy)proxy.Target;

			Assert.AreEqual(typeof(EventHandler), proxyTarget.ProxiedType);
			Assert.AreEqual(baseObject, baseObject);
			Assert.AreSame(invocationInterceptor, proxyTarget.InvocationInterceptor);
		}


		[Test]
		public void CreatingDelegateProxyFromNonDelegateTypeFails()
		{
            Assert.Throws<ArgumentException>(() => proxyFactory.CreateDelegateProxy<EmptyAbstractClass>(baseObject, invocationInterceptor));
		}

		[Test]
		public void InvokeDelegateProxy()
		{
			var proxy = proxyFactory.CreateDelegateProxy<EventHandler>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreSame(proxy.Target, invocation.Target);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(2, parameterValues.Count);
					Assert.AreEqual(this, parameterValues[0]);
					Assert.AreEqual(EventArgs.Empty, parameterValues[1]);					
				};

			proxy(this, EventArgs.Empty);

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}


		public abstract class EmptyAbstractClass
		{			
		}

		public interface IEmptyInterface
		{			
		}

		public interface IBaseInterfaceWithMethod
		{
			void Method();
		}

		public interface ISubInterface : IBaseInterfaceWithMethod
		{
		}

		public interface IInterfaceWithMethod
		{
			void Method();
		}

		public interface IInterfaceWithGenericMethod
		{
			TResult Method<T, TResult>(T obj);
		}

		public interface IInterfaceWithGenericMethodWithInterfaceTypeConstraints
		{
			TResult Method<T, TResult>(T obj) 
				where T : IComparable 
				where TResult : class, new();
		}

		public interface IInterfaceWithGenericMethodWithBaseTypeConstraint
		{
			TResult Method<T, TResult>(T obj)
				where T : BaseTypeConstraintClass;
		}


		public interface IGenericInterfaceWithMethod<T, TResult>
		{
			TResult Method(T obj);
		}

		public interface IInterfaceWithEvent
		{
			event EventHandler OnEvent;
		}

		public class BaseTypeConstraintClass
		{			
		}

		public class DerivedFromBaseTypeConstraintClass : BaseTypeConstraintClass
		{
		}
	}

	public abstract class ProxyFactoryTests<TArgument> : ProxyFactoryTestsBase
	{
		protected TArgument expectedInput;
		protected TArgument expectedInputIndex;
		protected TArgument expectedOutput;

 
		[Test]
		public void InvokeMethodOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithMethod).GetMethod("Method"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(expectedInput, parameterValues[0]);

					invocation.ReturnValue = expectedOutput;
				};



			var methodReturnValue = proxy.Method(expectedInput);

			Assert.AreEqual(expectedOutput, methodReturnValue);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeRefParameterMethodOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithRefParameterMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithRefParameterMethod).GetMethod("Method"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(expectedInput, parameterValues[0]);

					parameterValues[0] = expectedOutput;
				};


			var value = expectedInput;

			proxy.Method(ref value);
			Assert.AreEqual(expectedOutput, value);

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeOutParameterMethodOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithOutParameterMethod>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithOutParameterMethod).GetMethod("Method"), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(2, parameterValues.Count);
					Assert.AreEqual(expectedInput, parameterValues[0]);

					parameterValues[1] = expectedOutput;
				};


			TArgument outValue;

			proxy.Method(expectedInput, out outValue);
			Assert.AreEqual(expectedOutput, outValue);

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokePropertyGetterOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithProperty>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithProperty).GetProperty("Property").GetGetMethod(), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(0, parameterValues.Count);

					invocation.ReturnValue = expectedOutput;
				};


			var value = proxy.Property;

			Assert.AreEqual(expectedOutput, value);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokePropertySetterOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithProperty>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithProperty).GetProperty("Property").GetSetMethod(), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);					
					Assert.AreEqual(expectedInput, parameterValues[0]);
				};


			proxy.Property = expectedInput;

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeIndexedPropertyGetterOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithIndexedProperty>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithIndexedProperty).GetProperty("Item").GetGetMethod(), invocation.Method);
					
					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(1, parameterValues.Count);
					Assert.AreEqual(expectedInputIndex, parameterValues[0]);

					invocation.ReturnValue = expectedOutput;
				};


			var value = proxy[expectedInputIndex];

			Assert.AreEqual(expectedOutput, value);
			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}

		[Test]
		public void InvokeIndexedPropertySetterOnInterfaceProxy()
		{
			var proxy = proxyFactory.CreateInterfaceProxy<IInterfaceWithIndexedProperty>(baseObject, invocationInterceptor);

			invocationInterceptor.OnInvocationHandler =
				invocation =>
				{
					Assert.AreEqual(typeof(IInterfaceWithIndexedProperty).GetProperty("Item").GetSetMethod(), invocation.Method);

					var parameterValues = invocation.ParameterValues;

					Assert.AreEqual(2, parameterValues.Count);
					Assert.AreEqual(expectedInputIndex, parameterValues[0]);
					Assert.AreEqual(expectedInput, parameterValues[1]);
				};


			proxy[expectedInputIndex] = expectedInput;

			Assert.AreEqual(1, invocationInterceptor.InvocationCount);
		}



		public interface IInterfaceWithMethod
		{
			TArgument Method(TArgument value);
		}

		public interface IInterfaceWithRefParameterMethod
		{
			void Method(ref TArgument value);
		}

		public interface IInterfaceWithOutParameterMethod
		{
			void Method(TArgument input, out TArgument output);
		}

		public interface IInterfaceWithProperty
		{
			TArgument Property { get; set; }
		}
	
		public interface IInterfaceWithIndexedProperty
		{
			TArgument this[TArgument i] { get; set; }
		}


	}



}