using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Simple.Mocking.AcceptanceTests.Interfaces;

namespace Simple.Mocking.AcceptanceTests
{
    [TestFixture]
    public class AssertInvocationsWasMadeTests
    {
        [Test]
        public void ForSingleMock()
        {
            var myObject = Mock.Interface<IMyObject>();

            Expect.Once.MethodCall(() => myObject.MyMethod(1));
            Expect.Once.MethodCall(() => myObject.MyMethod(2));

            myObject.MyMethod(1);
            myObject.MyMethod(2);

            AssertInvocationsWasMade.MatchingExpectationsFor(myObject);
        }

        [Test]
        public void ForScope()
        {
            var expectationScope = new ExpectationScope();

            var myObject1 = Mock.Interface<IMyObject>(expectationScope);
            var myObject2 = Mock.Interface<IMyObject>(expectationScope);

            Expect.Once.MethodCall(() => myObject1.MyMethod(1));
            Expect.Once.MethodCall(() => myObject2.MyMethod(2));

            myObject1.MyMethod(1);
            myObject2.MyMethod(2);

            AssertInvocationsWasMade.MatchingExpectationsFor(expectationScope);
        }

        [Test]
        public void NotMet()
        {
            var myObject = Mock.Interface<IMyObject>();

            Expect.Once.MethodCall(() => myObject.MyMethod(1));


            var ex = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.MatchingExpectationsFor(myObject));

            Assert.That(ex.Message, Does.StartWith("All expectations has not been met, expected:"));
        }

        [Test]
        public void NumberOfCallsExceededButExceptionCatchedByUserCode()
        {
            var myObject = Mock.Interface<IMyObject>();

            Expect.Once.MethodCall(() => myObject.MyMethod(1));

            try
            {
                myObject.MyMethod(1);
                myObject.MyMethod(1);
            }
            catch (ExpectationsException)
            {
            }


            Assert.Throws<ExpectationsException>(() => AssertExpectations.IsMetFor(myObject));
        }

        [Test]
        public void ForStubInvocationAtLeastOnce()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);

            AssertInvocationsWasMade.AtLeastOnce.ForMethodCall(() => myObject.MyMethod(1));

            var ex = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.AtLeastOnce.ForMethodCall(() => myObject.MyMethod(2)));

            Assert.That(ex.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected 1..* actual 0:"));
        }

        [Test]
        public void ForStubInvocationAtLeastTwice()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(1);
            myObject.MyMethod(2);

            AssertInvocationsWasMade.AtLeast(2).ForMethodCall(() => myObject.MyMethod(1));

            var ex = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.AtLeast(2).ForMethodCall(() => myObject.MyMethod(2)));

            Assert.That(ex.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected 2..* actual 1:"));
        }

        [Test]
        public void ForStubInvocationAtMostOnce()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(2);

            AssertInvocationsWasMade.AtMostOnce.ForMethodCall(() => myObject.MyMethod(1));

            var ex = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.AtMostOnce.ForMethodCall(() => myObject.MyMethod(2)));

            Assert.That(ex.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected *..1 actual 2:"));
        }

        [Test]
        public void ForStubInvocationAtMostTwice()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(2);
            myObject.MyMethod(2);

            AssertInvocationsWasMade.AtMost(2).ForMethodCall(() => myObject.MyMethod(1));

            var ex = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.AtMost(2).ForMethodCall(() => myObject.MyMethod(2)));

            Assert.That(ex.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected *..2 actual 3:"));
        }

        [Test]
        public void ForStubInvocationExactlyOnce()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(2);

            AssertInvocationsWasMade.Once.ForMethodCall(() => myObject.MyMethod(1));

            var ex1 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Once.ForMethodCall(() => myObject.MyMethod(2)));
            var ex2 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Once.ForMethodCall(() => myObject.MyMethod(3)));

            Assert.That(ex1.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected 1 actual 2:"));
            Assert.That(ex2.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(3)', expected 1 actual 0:"));
        }

        [Test]
        public void ForStubInvocationExactlyTwice()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(3);
            myObject.MyMethod(3);
            myObject.MyMethod(3);

            AssertInvocationsWasMade.Exactly(2).ForMethodCall(() => myObject.MyMethod(1));

            var ex1 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Exactly(2).ForMethodCall(() => myObject.MyMethod(2)));
            var ex2 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Exactly(2).ForMethodCall(() => myObject.MyMethod(3)));

            Assert.That(ex1.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(2)', expected 2 actual 1:"));
            Assert.That(ex2.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(3)', expected 2 actual 3:"));
        }

        [Test]
        public void ForStubInvocationBetweenOneAndTwoTimes()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(3);
            myObject.MyMethod(3);
            myObject.MyMethod(3);

            AssertInvocationsWasMade.Between(1, 2).ForMethodCall(() => myObject.MyMethod(1));
            AssertInvocationsWasMade.Between(1, 2).ForMethodCall(() => myObject.MyMethod(2));

            var ex1 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Between(1, 2).ForMethodCall(() => myObject.MyMethod(3)));
            var ex2 = Assert.Throws<ExpectationsException>(() => AssertInvocationsWasMade.Between(1, 2).ForMethodCall(() => myObject.MyMethod(4)));

            Assert.That(ex1.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(3)', expected 1..2 actual 3:"));
            Assert.That(ex2.Message, Does.StartWith("Wrong number of invocations for 'myObject.MyMethod(4)', expected 1..2 actual 0:"));
        }

        [Test]
        public void ForStubInvocationsInOrderAsSpecified()
        {
            var myObject = Stub.Interface<IMyObject>();

            myObject.MyMethod(1);
            myObject.MyMethod(1);
            myObject.MyMethod(2);
            myObject.MyMethod(1);
            myObject.MyMethod(3);
            myObject.MyMethod(1);

            AssertInvocationsWasMade.
                AtLeastOnce.ForMethodCall(() => myObject.MyMethod(1)).                
                AtLeastOnce.ForMethodCall(() => myObject.MyMethod(2)). 
                AtLeastOnce.ForMethodCall(() => myObject.MyMethod(1)).
                AtLeastOnce.ForMethodCall(() => myObject.MyMethod(3)).
                AtLeastOnce.ForMethodCall(() => myObject.MyMethod(1)).
                InOrderAsSpecified();

            var ex =
                Assert.Throws<ExpectationsException>(
                    () => AssertInvocationsWasMade.
                              AtLeast(3).ForMethodCall(() => myObject.MyMethod(1)).
                              AtLeastOnce.ForMethodCall(() => myObject.MyMethod(2)).
                              AtLeastOnce.ForMethodCall(() => myObject.MyMethod(3)).
                              InOrderAsSpecified());

            Assert.That(ex.Message, Does.StartWith("Invocations was not made in specified order (first mismatch at invocation 'myObject.MyMethod(2)'):"));
 
        }
    }
}