using System;

using NUnit.Framework;

using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp.Proxies
{
    [TestFixture]
	public class InvocationFormatterTests
	{
		MyObject target = new MyObject();

		[Test]
		public void Method()
		{
			Assert.AreEqual("myObject.Method(1, 2)", 
				InvocationFormatter.Format(
					target, 
					typeof(IMyInterface).GetMethod("Method"), 
					new object?[]{ 1, 2 }));
		}

		[Test]
		public void GenericMethod()
		{
			Assert.AreEqual("myObject.GenericMethod<Int32, Int32>(1, 2)",
				InvocationFormatter.Format(
					target, 
					typeof(IMyInterface).GetMethod("GenericMethod")!.MakeGenericMethod(new []{ typeof(int), typeof(int)}), 
					new object?[] { 1, 2 }));
		}

		[Test]
		public void Delegate()
		{
			Assert.AreEqual("myObject(1, 2)",
				InvocationFormatter.Format(
					(MyDelegate)target.Method,
					typeof(MyObject).GetMethod("Method"),
					new object?[] { 1, 2 }));
		}

		[Test]
		public void PropertyGet()
		{
			Assert.AreEqual("myObject.Property",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Property")!.GetGetMethod(),
					new object?[0]));
		}

		[Test]
		public void PropertySet()
		{
			Assert.AreEqual("myObject.Property = 1",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Property")!.GetSetMethod(),
					new object?[] { 1 }));
		}

		[Test]
		public void PropertySetWildcard()
		{
			Assert.AreEqual("myObject.Property = *",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Property")!.GetSetMethod(),
					null));
		}

		[Test]
		public void IndexedPropertyGet()
		{
			Assert.AreEqual("myObject[1, 2]",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Item")!.GetGetMethod(),
					new object?[] { 1, 2 }));
		}

		[Test]
		public void IndexedPropertyGetWildcard()
		{
			Assert.AreEqual("myObject[*]",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Item")!.GetGetMethod(),
					null));
		}

		[Test]
		public void IndexedPropertySet()
		{
			Assert.AreEqual("myObject[1, 2] = 3",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Item")!.GetSetMethod(),
					new object?[] { 1, 2, 3 }));
		}

		[Test]
		public void IndexedPropertySetWildcard()
		{
			Assert.AreEqual("myObject[*] = *",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetProperty("Item")!.GetSetMethod(),
					null));
		}

		[Test]
		public void EventAdd()
		{
			Assert.AreEqual("myObject.Event += System.EventHandler",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetEvent("Event")!.GetAddMethod(),
					new object?[] { (EventHandler)target.EventHandler }));
		}

		[Test]
		public void EventRemove()
		{
			Assert.AreEqual("myObject.Event -= System.EventHandler",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetEvent("Event")!.GetRemoveMethod(),
					new object?[] { (EventHandler)target.EventHandler }));
		}

		[Test]
		public void StringParametersAreFormatted()
		{
			Assert.AreEqual("myObject.MethodWithStringArgument(\"Hello World\\n\")",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetMethod("MethodWithStringArgument"),
					new object?[] { "Hello World\n" }));
		}

		[Test]
		public void CharParametersAreFormatted()
		{
			Assert.AreEqual("myObject.MethodWithCharArgument('\\t')",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetMethod("MethodWithCharArgument"),
					new object?[] { '\t' }));
		}

		[Test]
		public void NullParametersAreFormatted()
		{
			Assert.AreEqual("myObject.MethodWithObjectArgument(null)",
				InvocationFormatter.Format(
					target,
					typeof(IMyInterface).GetMethod("MethodWithObjectArgument"),
					new object?[] { null }));
		}


		delegate int MyDelegate(int a, int b);

		class MyObject
		{
			public int Method(int a, int b)
			{
				return a + b;
			}

			public void EventHandler(object? sender, EventArgs e)
			{				
			}

			public override string ToString()
			{
				return "myObject";
			}
		}

		interface IMyInterface
		{
			void MethodWithStringArgument(string a);
			void MethodWithCharArgument(char a);
			void MethodWithObjectArgument(object a);
			int Method(int a, int b);
			int GenericMethod<A, B>(A a, B b);			
			int Property { get; set; }			
			int this[int i, int j] { get; set; }
			event EventHandler Event;
		}
	}
}