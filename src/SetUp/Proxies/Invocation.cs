using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Mocking.SetUp.Proxies
{
    public class Invocation : IInvocation
	{
		IProxy target;
		MethodInfo method;
		IList<Type>? genericArguments;
		IList<object?> parameterValues;
		object? returnValue;
	    long invocationOrder;

		internal Invocation(IProxy target, MethodInfo method, IList<Type>? genericArguments, IList<object?> parameterValues, object? returnValue, long invocationOrder)
		{
			this.target = target;
			this.method = method;
			this.genericArguments = (genericArguments != null ? new GenericArgumentsList(genericArguments) : null);
			this.parameterValues = new ParameterList(this, parameterValues);
			this.returnValue = returnValue;
		    this.invocationOrder = invocationOrder;
		}

		public IProxy Target => target;

		public MethodInfo Method => method;

		public IList<Type>? GenericArguments => genericArguments;

		public IList<object?> ParameterValues => parameterValues;

		public object? ReturnValue
		{
			internal get => returnValue;
			set
			{
				AssertMethodReturnValueIsAssignable(value);
				returnValue = value;
			}
		}

	    public long InvocationOrder => invocationOrder;


	    public static object? HandleInvocation(
			IProxy target, InvocationFactory invocationFactory,
			Type[]? genericArguments, object?[] parameterValues, object? returnValue)
		{
			var invocation = invocationFactory.CreateInvocation(target, genericArguments, parameterValues, returnValue);

			var invocationInterceptor = target.InvocationInterceptor;

			invocationInterceptor.OnInvocation(invocation);

			return invocation.returnValue;
		}

		void AssertMethodParameterIsAssignable(int index, object? value)
		{
			var parameterType = GetNonGenericMethod(this).GetParameters()[index].ParameterType;

			if (!parameterType.IsByRef)
			{
				throw new InvalidOperationException(
					string.Format("Can not set parameter {0} of method '{1}' (not an out or ref parameter)", index, method));
			}

			if (!parameterType.GetElementType()!.IsAssignable(value))
			{
				throw new InvalidOperationException(
					string.Format("Can not set parameter {0} of method '{1}' (value '{2}' is not assignable)", index, method, value));				
			}
		}		

		void AssertMethodReturnValueIsAssignable(object? value)
		{
			var returnType = GetNonGenericMethod(this).ReturnType;

			if (returnType == typeof(void))
			{
				throw new InvalidOperationException(
					string.Format("Can not return value from  method '{0}' (return type is void)", method));
			}

			if (!returnType.IsAssignable(value))
			{
				throw new InvalidOperationException(
					string.Format("Can not return value of method '{0}' (value '{1}' is not assignable)", method, value));
			}
		}

	    public override string ToString() => InvocationFormatter.Format(target, method, genericArguments, parameterValues);


        public static MethodInfo GetNonGenericMethod(IInvocation invocation)
        {
            var method = invocation.Method;
            var genericArguments = invocation.GenericArguments;

            return (genericArguments != null ? method.MakeGenericMethod(genericArguments.ToArray()) : method);
        }




		class ListWithRestrictedAccess<T> : IList<T>
		{
			private IList<T> list;

			protected ListWithRestrictedAccess(IList<T> list)
			{
				this.list = list;
			}

			public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public virtual void Add(T item) => throw new NotSupportedException();

			public virtual void Clear() => throw new NotSupportedException();

			public bool Contains(T item) => list.Contains(item);

			public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

			public virtual bool Remove(T item) => throw new NotSupportedException();

			public int Count => list.Count;

			public virtual bool IsReadOnly => list.IsReadOnly;
			
			public int IndexOf(T item) => list.IndexOf(item);

			public virtual void Insert(int index, T item) => throw new NotSupportedException();

			public virtual void RemoveAt(int index) => throw new NotSupportedException();

			public virtual T this[int index]
			{
				get => list[index];
				set => SetItem(index, value);
			}

			protected virtual void SetItem(int index, T value) => list[index] = value;
		}
		
		class GenericArgumentsList : ListWithRestrictedAccess<Type>
		{
			public GenericArgumentsList(IList<Type> list) : base(list)
			{
			}

			protected override void SetItem(int index, Type value) => throw new NotSupportedException();
		}

		class ParameterList : ListWithRestrictedAccess<object?>
		{
			Invocation invocation;

			public ParameterList(Invocation invocation, IList<object?> list)
				: base(list)
			{
				this.invocation = invocation;
			}

			protected override void SetItem(int index, object? value)
			{
				invocation.AssertMethodParameterIsAssignable(index, value);
				base.SetItem(index, value);
			}
		}
	}
}