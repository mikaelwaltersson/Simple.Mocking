using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Delegates;
using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
    [TestFixture]
    public class StubTests
    {
        [Test]
        public void StubInterface()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(3);


            AssertInvocationsWasMade.
                Once.ForMethodCall(() => myObject.MyMethod(1)).
                Between(1, 2).ForMethodCall(() => myObject.MyMethod(2)).
                Once.ForMethodCall(() => myObject.MyMethod(3)).
                InOrderAsSpecified();


            AssertInvocationsWasMade.MatchingExpectationsFor(myObject);

            AssertInvocationsWasMade.None.ForMethodCall(() => myObject.MyGenericMethod(0));


            Assert.That(myObject.MyProperty, Is.TypeOf<object>());

            Assert.That(myObject.MyIntProperty, Is.EqualTo(0));

            int outValue;
            myObject.MyMethodWithOutParameter(0, out outValue);
            Assert.That(outValue, Is.EqualTo(0));

        }

        [Test]
        public void StubDelegate()
        {
            var myDelegate = Stub.Delegate<MyDelegate>();

            myDelegate(1);
            myDelegate(2);
            myDelegate(3);

            AssertInvocationsWasMade.
                Exactly(3).ForMethodCall(() => myDelegate(Any<int>.Value));
        }

        [Test]
        public void StubMethods()
        {
            var myObject = Stub.Interface<IMyObject>();

            Stub.MethodCall(() => myObject.MyMethodWithReturnValue(Any<int>.Value)).Returns(42);
            Stub.MethodCall(() => myObject.MyMethodWithOutParameter(Any<int>.Value, out Any<int>.Value.AsRefOrOut)).SetsOutOrRefParameter(1, 42);

            
            var result1 = myObject.MyMethodWithReturnValue(12345);

            int result2;
            myObject.MyMethodWithOutParameter(12345, out result2);


            Assert.AreEqual(42, result1);
            Assert.AreEqual(42, result2);
        }

        [Test]
        public void StubPropertyGets()
        {
            var myObject = Stub.Interface<IMyObject>();

            Stub.PropertyGet(() => myObject.MyIntProperty).Returns(42);
            Stub.PropertyGet(() => myObject.MyProperty).Returns("Hello World");

            Assert.AreEqual(42, myObject.MyIntProperty);
            Assert.AreEqual("Hello World", myObject.MyProperty);
        }
    }
}
