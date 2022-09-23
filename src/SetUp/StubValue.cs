using System;
using System.Linq;

namespace Simple.Mocking.SetUp
{
    static class StubValue
    {
        public static object? ForType(Type type)
        {
            if (type.IsInterface)
                return CreateStub(typeof(InterfaceStubFactory<>), type);

            if (typeof(Delegate).IsAssignableFrom(type))
                return CreateStub(typeof(DelegateStubFactory<>), type);

            if (IsConcreteClassWithPublicEmptyConstructor(type))
                return Activator.CreateInstance(type)!;

            if (type == typeof(string))
                return string.Empty;

            if (type.IsArray)
                return GetEmptyArray(type);

            return GetDefaultValueForType(type);
        }

        static bool IsConcreteClassWithPublicEmptyConstructor(Type type) =>
            type.IsClass && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null;

        static object CreateStub(Type genericStubFactoryType, Type type) =>
            ((IStubFactory)Activator.CreateInstance(genericStubFactoryType.MakeGenericType(type))!).CreateStub();

        static object? GetDefaultValueForType(Type type) =>
            ((IDefaultValue)Activator.CreateInstance(typeof(DefaultValue<>).MakeGenericType(type))!).Value;

        static Array GetEmptyArray(Type type) =>
            Array.CreateInstance(type.GetElementType()!, Enumerable.Repeat(0, type.GetArrayRank()).ToArray());

        interface IStubFactory
        {
            object CreateStub();
        }

        class InterfaceStubFactory<T> : IStubFactory where T : notnull
        {
            public object CreateStub() =>
                Stub.Interface<T>();
        }

        class DelegateStubFactory<T> : IStubFactory where T : notnull
        {
            public object CreateStub() =>
                Stub.Delegate<T>();
        }

        interface IDefaultValue
        {
            object? Value { get; }
        }

        class DefaultValue<T> : IDefaultValue
        {
            public object? Value => default(T);
        }
    }
}
