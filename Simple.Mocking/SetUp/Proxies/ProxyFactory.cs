using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;


namespace Simple.Mocking.SetUp.Proxies
{
	class ProxyFactory
	{
		const string ModuleAndAssemblyNameFormat = "Proxies-{0}";
		const string TypeNameFormat = "Proxies-{0}";
		const string MethodNameFormat = "{0}.{1}";
		const char TypeNameInterfaceFullNameDelimiter = '-';
		const string InvocationFactoryFieldNameFormat = "invocationFactory{0}";
		const string DelegateInvokeMethodName = "Invoke";

		ModuleBuilder moduleBuilder;
		ProxyTypeCache proxyTypeCache;



		public ProxyFactory()
			: this(CreateModuleBuilder())
		{
		}

		public ProxyFactory(ModuleBuilder moduleBuilder)
		{
			if (moduleBuilder == null)
				throw new ArgumentNullException("moduleBuilder");

			this.moduleBuilder = moduleBuilder;
			this.proxyTypeCache = new ProxyTypeCache();
		}


		static ModuleBuilder CreateModuleBuilder()
		{
			string moduleAndAssemblyName = string.Format(ModuleAndAssemblyNameFormat, Guid.NewGuid());

			var assemblyName = new AssemblyName(moduleAndAssemblyName);

			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

			return assemblyBuilder.DefineDynamicModule(moduleAndAssemblyName);
		}

		public T CreateInterfaceProxy<T>(object baseObject, IInvocationInterceptor invocationInterceptor)
		{
			ProxyBase<T>.CheckConstructorArguments(baseObject, invocationInterceptor);

			var interfaceType = typeof(T);

			if (!interfaceType.IsInterface)
				throw new ArgumentException(string.Format("{0} is not an interface", interfaceType));

			var proxyType = proxyTypeCache.GetProxyType(interfaceType, CreateProxyTypeForInterface);

			return (T)CreateInstance(proxyType, baseObject, invocationInterceptor);
		}
		
		public T CreateDelegateProxy<T>(object baseObject, IInvocationInterceptor invocationInterceptor)
		{
			ProxyBase<T>.CheckConstructorArguments(baseObject, invocationInterceptor);

			var delegateType = typeof(T);

			if (!delegateType.IsDelegateType())
				throw new ArgumentException(string.Format("{0} is not an delegate", delegateType));

			var proxyType = proxyTypeCache.GetProxyType(delegateType, CreateProxyTypeForDelegate);

			return (T)CreateDelegate(proxyType, delegateType, baseObject, invocationInterceptor);
		}

		object CreateInstance(Type proxyType, params object[] constructorArguments)
		{
			return Activator.CreateInstance(proxyType, constructorArguments);
		}

		object CreateDelegate(Type proxyType, Type delegateType, params object[] constructorArguments)
		{
			var target = CreateInstance(proxyType, constructorArguments);

			return Delegate.CreateDelegate(delegateType, target, DelegateInvokeMethodName);
		}


		Type CreateProxyTypeForInterface(Type interfaceType)
		{
			return CreateProxyType(interfaceType, new[] { interfaceType });
		}

		Type CreateProxyTypeForDelegate(Type delegateType)
		{
			return CreateProxyType(delegateType, new Type[0]);
		}

		Type CreateProxyType(Type proxiedType, Type[] implementedInterfaceTypes)
		{
			var baseType = typeof(ProxyBase<>).MakeGenericType(proxiedType);

			string typeName = GetProxyTypeName(proxiedType);
			var typeAttributes = TypeAttributes.Class | TypeAttributes.Public;
			var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, baseType, implementedInterfaceTypes);

			ImplementMembers(typeBuilder, baseType, proxiedType);

			return typeBuilder.CreateTypeInfo();
		}


		void ImplementMembers(TypeBuilder typeBuilder, Type baseType, Type interfaceType)
		{
			var baseConstructor = GetBaseConstructor(baseType);

			var baseMethods = GetBaseMethods(interfaceType);

			var invocationFactoryFields = ImplementClassConstructor(typeBuilder, baseMethods);

			ImplementConstructor(typeBuilder, baseConstructor);
			ImplementMethods(typeBuilder, baseMethods, invocationFactoryFields);
		}

		MethodInfo[] GetBaseMethods(Type interfaceType)
		{
			if (interfaceType.IsDelegateType())
				return new[] { interfaceType.GetMethod(DelegateInvokeMethodName) };

			var interfaceMethods = GetInterfaceMethods(interfaceType);
			var objectMethods = GetVirtualMethodsDeclaredInObject();

			return interfaceMethods.Concat(objectMethods).ToArray();
		}



		ConstructorInfo GetBaseConstructor(Type baseType)
		{
			return baseType.GetConstructor(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
				null, 
				new[] { typeof(object), typeof(IInvocationInterceptor) }, 
				null);
		}

		MethodInfo[] GetInterfaceMethods(Type interfaceType)
		{
			var interfaceMethods = new List<MethodInfo>(interfaceType.GetMethods());

			foreach (Type subInterfaceType in interfaceType.GetInterfaces())
				interfaceMethods.AddRange(GetInterfaceMethods(subInterfaceType));

			return interfaceMethods.Distinct().ToArray();
		}

		MethodInfo[] GetVirtualMethodsDeclaredInObject()
		{
			return typeof(object).GetMethods().Where(method => method.IsVirtual).ToArray();
		}


		FieldInfo[] ImplementClassConstructor(TypeBuilder typeBuilder, MethodInfo[] baseMethods)
		{
			var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, null);

			var ilGenerator = constructorBuilder.GetILGenerator();

			var invocationFactoryFields = new FieldInfo[baseMethods.Length];

			for (int i = 0; i < baseMethods.Length; i++)
			{
				var baseMethod = baseMethods[i];
				var declaringType = baseMethod.DeclaringType;

				var invocationFactoryField = ImplementInvocationFactoriesField(typeBuilder, i);

				if (baseMethod.IsGenericMethodDefinition)
				{					
					string methodTextRepresentation = InvocationFactory.GetTextRepresentationForMethod(baseMethod);

					ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
					ilGenerator.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
					ilGenerator.Emit(OpCodes.Ldstr, methodTextRepresentation);
					ilGenerator.Emit(OpCodes.Call, Methods.GetInvocationFactoryForMethodTextRepresentation);
					ilGenerator.Emit(OpCodes.Stsfld, invocationFactoryField);
				}
				else
				{
					ilGenerator.Emit(OpCodes.Ldtoken, baseMethod);
					ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
					ilGenerator.Emit(OpCodes.Call, Methods.GetMethodFromHandle);
					ilGenerator.Emit(OpCodes.Call, Methods.GetInvocationFactoryForMethod);
					ilGenerator.Emit(OpCodes.Stsfld, invocationFactoryField);
				}

				invocationFactoryFields[i] = invocationFactoryField;
			}

			ilGenerator.Emit(OpCodes.Ret);

			return invocationFactoryFields;
		}

		FieldInfo ImplementInvocationFactoriesField(TypeBuilder typeBuilder, int fieldIndex)
		{
			string fieldName = string.Format(InvocationFactoryFieldNameFormat, fieldIndex);

			var fieldType = typeof(InvocationFactory);
			var fieldAttributes = FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly;

			return typeBuilder.DefineField(fieldName, fieldType, fieldAttributes);
		}

		void ImplementConstructor(TypeBuilder typeBuilder, ConstructorInfo baseConstructor)
		{
			var methodAttributes = MethodAttributes.Public;
			var callingConvention = baseConstructor.CallingConvention;
			
			var parameterTypes = (from parameter in baseConstructor.GetParameters() select parameter.ParameterType).ToList();

			var constructorBuilder = typeBuilder.DefineConstructor(methodAttributes, callingConvention, parameterTypes.ToArray());

			var ilGenerator = constructorBuilder.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			for (int i = 0; i < parameterTypes.Count; i++)
				ilGenerator.Emit(OpCodes.Ldarg, i + 1);

			ilGenerator.Emit(OpCodes.Call, baseConstructor);
			ilGenerator.Emit(OpCodes.Ret);
		}

		void ImplementMethods(TypeBuilder typeBuilder, MethodInfo[] baseMethods, FieldInfo[] invocationFactoryFields)
		{
			for (int i = 0; i < baseMethods.Length; i++)
			{
				var baseMethod = baseMethods[i];
				var invocationFactoryField = invocationFactoryFields[i];

				ImplementMethod(typeBuilder, baseMethod, invocationFactoryField);
			}
		}

		void ImplementMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, FieldInfo invocationFactoryField)
		{
			string methodName;			
			var methodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot;
			bool isMethodOverride;


			if (baseMethod.DeclaringType.IsDelegateType())
			{
				methodName = DelegateInvokeMethodName;
				methodAttributes |= MethodAttributes.Public;
				isMethodOverride = false;
			}
			else
			{
				methodName = GetProxyMethodName(baseMethod);
				methodAttributes |= MethodAttributes.Private;
				isMethodOverride = true;
			}

			var methodBuilder = typeBuilder.DefineMethod(methodName, methodAttributes);

			DefineMethodSignature(methodBuilder, baseMethod);

			EmitMethodBody(methodBuilder, baseMethod, invocationFactoryField);

			if (isMethodOverride)
				typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
		}

		void DefineMethodSignature(MethodBuilder methodBuilder, MethodInfo baseMethod)
		{
			if (baseMethod.IsGenericMethodDefinition)
			{
				var baseGenericArguments = baseMethod.GetGenericArguments();
				var genericParameterBuilders = methodBuilder.DefineGenericParameters(baseGenericArguments.Select(type => type.Name).ToArray());

				for (int i = 0; i < genericParameterBuilders.Length; i++)
				{
					var baseGenericArgument = baseGenericArguments[i];
					var genericParameterBuilder = genericParameterBuilders[i];

					foreach (var baseTypeConstraint in baseGenericArgument.GetGenericParameterConstraints())
					{
						if (baseTypeConstraint.IsInterface)
							genericParameterBuilder.SetInterfaceConstraints(baseTypeConstraint);
						else
							genericParameterBuilder.SetBaseTypeConstraint(baseTypeConstraint);
					}
				}
			}

			var baseMethodParameters = baseMethod.GetParameters();

			methodBuilder.SetParameters(baseMethodParameters.Select(parameter => parameter.ParameterType).ToArray());
			methodBuilder.SetReturnType(baseMethod.ReturnType);

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				var parameter = baseMethodParameters[i];

				methodBuilder.DefineParameter(i, parameter.Attributes, parameter.Name);
			}
		}

		void EmitMethodBody(MethodBuilder methodBuilder, MethodInfo baseMethod, FieldInfo invocationFactoryField)
		{
			var ilGenerator = methodBuilder.GetILGenerator();

			ilGenerator.DeclareLocal(typeof(InvocationFactory));
			ilGenerator.DeclareLocal(typeof(Type[]));
			ilGenerator.DeclareLocal(typeof(object[]));

			if (baseMethod.ReturnType != typeof(void))
				ilGenerator.DeclareLocal(baseMethod.ReturnType);

			EmitLoadInvocationFactoriesField(ilGenerator, invocationFactoryField);

			EmitBuildGenericArgumentsArray(ilGenerator, baseMethod);

			EmitBuildParameterValuesArray(ilGenerator, baseMethod);

			EmitCallHandleInvocation(ilGenerator, baseMethod);

			EmitReflectOutgoingParameterValues(ilGenerator, baseMethod);

			ilGenerator.Emit(OpCodes.Ret);
		}

		void EmitLoadInvocationFactoriesField(ILGenerator ilGenerator, FieldInfo invocationFactoriesField)
		{
			ilGenerator.Emit(OpCodes.Ldsfld, invocationFactoriesField);
			ilGenerator.Emit(OpCodes.Stloc_0);
		}

		void EmitBuildGenericArgumentsArray(ILGenerator ilGenerator, MethodInfo baseMethod)
		{
			if (!baseMethod.IsGenericMethodDefinition)
			{
				ilGenerator.Emit(OpCodes.Ldnull);
				ilGenerator.Emit(OpCodes.Stloc_1);
				return;
			}

			var genericMethodArguments = baseMethod.GetGenericArguments();

			ilGenerator.Emit(OpCodes.Ldc_I4, genericMethodArguments.Length);
			ilGenerator.Emit(OpCodes.Newarr, typeof(Type));
			ilGenerator.Emit(OpCodes.Stloc_1);

			for (int i = 0; i < genericMethodArguments.Length; i++)
			{
				ilGenerator.Emit(OpCodes.Ldloc_1);
				ilGenerator.Emit(OpCodes.Ldc_I4, i);
				ilGenerator.Emit(OpCodes.Ldtoken, genericMethodArguments[i]);
				ilGenerator.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
				ilGenerator.Emit(OpCodes.Stelem_Ref);
			}
		}

		void EmitBuildParameterValuesArray(ILGenerator ilGenerator, MethodInfo baseMethod)
		{
			var baseMethodParameters = baseMethod.GetParameters();

			ilGenerator.Emit(OpCodes.Ldc_I4, baseMethodParameters.Length);
			ilGenerator.Emit(OpCodes.Newarr, typeof(object));
			ilGenerator.Emit(OpCodes.Stloc_2);

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				var parameter = baseMethodParameters[i];

				var parameterType = parameter.ParameterType;

				ilGenerator.Emit(OpCodes.Ldloc_2);
				ilGenerator.Emit(OpCodes.Ldc_I4, i);

				ilGenerator.Emit(OpCodes.Ldarg, i + 1);

				if (parameterType.IsByRef)
				{
					parameterType = parameterType.GetElementType();
					ilGenerator.EmitLoadIndirect(parameterType);
				}

				EmitBoxValue(ilGenerator, parameterType);

				ilGenerator.Emit(OpCodes.Stelem_Ref);
			}
		}

		void EmitLoadDefaultReturnValue(ILGenerator ilGenerator, Type returnType)
		{
			ilGenerator.Emit(OpCodes.Ldloca_S, 3);
			ilGenerator.Emit(OpCodes.Initobj, returnType);

			ilGenerator.Emit(OpCodes.Ldloc_3);
			EmitBoxValue(ilGenerator, returnType);
		}

		void EmitBoxValue(ILGenerator ilGenerator, Type parameterType)
		{
			if (parameterType.IsValueType || parameterType.IsGenericParameter)
				ilGenerator.Emit(OpCodes.Box, parameterType);
		}

		void EmitCallHandleInvocation(ILGenerator ilGenerator, MethodInfo baseMethod)
		{
			ilGenerator.Emit(OpCodes.Ldarg_0);					
			ilGenerator.Emit(OpCodes.Ldloc_0);			
			ilGenerator.Emit(OpCodes.Ldloc_1);
			ilGenerator.Emit(OpCodes.Ldloc_2);

			if (baseMethod.ReturnType == typeof(void))
			{
				ilGenerator.Emit(OpCodes.Ldnull);
				ilGenerator.Emit(OpCodes.Call, Methods.HandleInvocation);
				ilGenerator.Emit(OpCodes.Pop);
			}
			else
			{				
				EmitLoadDefaultReturnValue(ilGenerator, baseMethod.ReturnType);
				ilGenerator.Emit(OpCodes.Call, Methods.HandleInvocation);
				ilGenerator.Emit(OpCodes.Unbox_Any, baseMethod.ReturnType);
			}				
		}

		void EmitReflectOutgoingParameterValues(ILGenerator ilGenerator, MethodInfo baseMethod)
		{
			var baseMethodParameters = baseMethod.GetParameters();

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				var parameter = baseMethodParameters[i];
				var parameterType = parameter.ParameterType;

				if (!parameterType.IsByRef)
					continue;

				parameterType = parameterType.GetElementType();

				ilGenerator.Emit(OpCodes.Ldarg, i + 1);
				ilGenerator.Emit(OpCodes.Ldloc_2);
				ilGenerator.Emit(OpCodes.Ldc_I4, i);
				ilGenerator.Emit(OpCodes.Ldelem_Ref);
				ilGenerator.Emit(OpCodes.Unbox_Any, parameterType);
				ilGenerator.EmitStoreIndirect(parameterType);
			}
		}



		static string GetProxyTypeName(Type type)
		{
			return string.Format(TypeNameFormat,
				type.FullName.Replace(Type.Delimiter, TypeNameInterfaceFullNameDelimiter));
		}

		static string GetProxyMethodName(MethodInfo method)
		{
			return string.Format(MethodNameFormat,
				method.DeclaringType.FullName.Replace(Type.Delimiter, TypeNameInterfaceFullNameDelimiter), method.Name);
		}


		static class Methods
		{		
			public static readonly MethodInfo GetTypeFromHandle =
				GetMethod<Func<RuntimeTypeHandle, Type>>(Type.GetTypeFromHandle);

			public static readonly MethodInfo GetMethodFromHandle =
				GetMethod<Func<RuntimeMethodHandle, RuntimeTypeHandle, MethodBase>>(MethodBase.GetMethodFromHandle);
      
			public static readonly MethodInfo HandleInvocation =
				GetMethod<HandleInvocationCallback>(Invocation.HandleInvocation);

			public static readonly MethodInfo GetInvocationFactoryForMethod =
				GetMethod<Func<MethodInfo, InvocationFactory>>(InvocationFactory.GetForMethod);

			public static readonly MethodInfo GetInvocationFactoryForMethodTextRepresentation =
				GetMethod<Func<Type, string, InvocationFactory>>(InvocationFactory.GetForMethodTextRepresentation);

			static MethodInfo GetMethod<TFunc>(TFunc func)
			{
				return ((Delegate)(object)func).Method;
			}

		}

		delegate object HandleInvocationCallback(
			IProxy target, InvocationFactory invocationFactory,
			Type[] genericArguments, object[] parameterValues, object returnValue);

	}
}

