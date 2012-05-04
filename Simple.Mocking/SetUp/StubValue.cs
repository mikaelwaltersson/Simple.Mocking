using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.SetUp
{
    static class StubValue
    {
        public static object ForType(Type type)
        {
            if (type.IsInterface)
                return CreateStub(typeof(InterfaceStubFactory<>), type);

            if (typeof(Delegate).IsAssignableFrom(type))
                return CreateStub(typeof(DelegateStubFactory<>), type);

            if (IsConcreteClassWithPublicEmptyConstructor(type))
                return Activator.CreateInstance(type);

            if (type == typeof(string))
                return string.Empty;

            return GetDefaultValueForType(type);
        }

        static bool IsConcreteClassWithPublicEmptyConstructor(Type type)
        {
            return type.IsClass && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null;
        }

        static object CreateStub(Type genericStubFactoryType, Type type)
        {
            return ((IStubFactory)Activator.CreateInstance(genericStubFactoryType.MakeGenericType(type))).CreateStub();
        }

        static object GetDefaultValueForType(Type type)
        {
            return ((IDefaultValue)Activator.CreateInstance(typeof(DefaultValue<>).MakeGenericType(type))).Value;
        }

        interface IStubFactory
        {
            object CreateStub();
        }

        class InterfaceStubFactory<T> : IStubFactory
        {
            public object CreateStub()
            {
                return Stub.Interface<T>();
            }
        }

        class DelegateStubFactory<T> : IStubFactory
        {
            public object CreateStub()
            {
                return Stub.Delegate<T>();
            }
        }

        interface IDefaultValue
        {
            object Value { get; }
        }

        class DefaultValue<T> : IDefaultValue
        {
            public object Value
            {
                get { return default(T); }
            }
        }

    }
}
