using System;
using System.Collections.Generic;

using NUnit.Framework;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking.UnitTests.SetUp
{
    [TestFixture]
    public class StubValueTests
    {
        [Test]
        public void StubValuesForValueTypes()
        {
            Assert.AreEqual(0, StubValue.ForType(typeof(int)));
            Assert.AreEqual(0.0, StubValue.ForType(typeof(double)));
            Assert.AreEqual(0m, StubValue.ForType(typeof(decimal)));
        }

        [Test]
        public void StubValuesForReferenceTypesWithDefaultConstructors()
        {
            Assert.That(StubValue.ForType(typeof(List<int>)), Is.Not.Null.And.TypeOf<List<int>>());
        }

        [Test]
        public void StubValuesForReferenceTypesWithoutDefaultConstructors()
        {
            Assert.That(StubValue.ForType(typeof(Uri)), Is.Null);
        }

        [Test]
        public void StubValuesForStrings()
        {
            Assert.That(StubValue.ForType(typeof(string)), Is.EqualTo(string.Empty));
        }

        [Test]
        public void StubValuesForArrays()
        {
            Assert.That(StubValue.ForType(typeof(int[])), Is.Empty);
            Assert.That(StubValue.ForType(typeof(double[,])), Is.Empty);
            Assert.That(StubValue.ForType(typeof(string[,,])), Is.Empty);
        }


        [Test]
        public void StubValuesForInterface()
        {
            var value = StubValue.ForType(typeof(IMyInterface))!;

            Assert.That(value, Is.InstanceOf<IProxy>());
            Assert.That(((IProxy)value).BaseObject, Is.TypeOf<Mock>());
        }

        [Test]
        public void StubValuesForDelegate()
        {
            var value = StubValue.ForType(typeof(Action))!;

            Assert.That(value, Is.InstanceOf<Delegate>());
            Assert.That(((Delegate)value).Target, Is.InstanceOf<IProxy>());
            Assert.That(((IProxy)((Delegate)value).Target!).BaseObject, Is.TypeOf<Mock>());
        }

        public interface IMyInterface
        {             
        }
    }
}
