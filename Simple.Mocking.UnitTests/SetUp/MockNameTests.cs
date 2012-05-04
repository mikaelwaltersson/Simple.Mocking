using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Simple.Mocking.SetUp;

namespace Simple.Mocking.UnitTests.SetUp
{
    [TestFixture]
    public class MockNameTests
    {
        [Test]
        public void GetUniqueName()
        {
            var scope = new Scope();

            Assert.AreEqual("object", MockName<object>.GetUniqueInScope(scope));
            Assert.AreEqual("object2", MockName<object>.GetUniqueInScope(scope));
            Assert.AreEqual("object3", MockName<object>.GetUniqueInScope(scope));

            Assert.AreEqual("string", MockName<string>.GetUniqueInScope(scope));
            Assert.AreEqual("string2", MockName<string>.GetUniqueInScope(scope));
            Assert.AreEqual("string3", MockName<string>.GetUniqueInScope(scope));

            Assert.AreEqual("iMyDelegate", MockName<IMyDelegate>.GetUniqueInScope(scope));
            Assert.AreEqual("imyDelegate", MockName<IMYDelegate>.GetUniqueInScope(scope));
            Assert.AreEqual("myInterface", MockName<IMyInterface>.GetUniqueInScope(scope));

            Assert.AreEqual("myGenericInterfaceWithOneArg", MockName<IMyGenericInterfaceWithOneArg<string>>.GetUniqueInScope(scope));
            Assert.AreEqual("myGenericInterfaceWithTwoArg", MockName<IMyGenericInterfaceWithTwoArg<string, int>>.GetUniqueInScope(scope));
        }

        delegate void IMYDelegate();

        delegate void IMyDelegate();

        interface IMyInterface
        {
        }

        interface IMyGenericInterfaceWithOneArg<T1>
        {
        }

        interface IMyGenericInterfaceWithTwoArg<T1, T2>
        {
        }

        class Scope : IMockNameScope
        {
            HashSet<string> names = new HashSet<string>();

            public bool Register(string name)
            {
                return names.Add(name);
            }
        }
    }
}