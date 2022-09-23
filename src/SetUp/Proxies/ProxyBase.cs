using System;

namespace Simple.Mocking.SetUp.Proxies
{
    public abstract class ProxyBase<T> : IProxy
	{
		object baseObject;
		IInvocationInterceptor invocationInterceptor;

		internal static void CheckConstructorArguments(object baseObject, IInvocationInterceptor invocationInterceptor)
		{
			if (baseObject == null)
				throw new ArgumentNullException("baseObject");

			if (invocationInterceptor == null)
				throw new ArgumentNullException("invocationInterceptor");
		}

		protected ProxyBase(object baseObject, IInvocationInterceptor invocationInterceptor)
		{
			CheckConstructorArguments(baseObject, invocationInterceptor);

			this.baseObject = baseObject;
			this.invocationInterceptor = invocationInterceptor;
		}

		public object BaseObject => baseObject;

		public Type ProxiedType => typeof(T);

		public IInvocationInterceptor InvocationInterceptor => invocationInterceptor;
	}
}
