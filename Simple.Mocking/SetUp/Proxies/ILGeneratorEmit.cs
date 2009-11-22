using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Simple.Mocking.SetUp.Proxies
{
	static class ILGeneratorEmit
	{
		public static void EmitStoreIndirect(this ILGenerator ilGenerator, Type type)
		{
			if (type.IsEnum)
				type = Enum.GetUnderlyingType(type);

			if (type.IsValueType)
			{
				if (type == typeof(bool)) ilGenerator.Emit(OpCodes.Stind_I1);
				else if (type == typeof(char)) ilGenerator.Emit(OpCodes.Stind_I2);
				else if (type == typeof(float)) ilGenerator.Emit(OpCodes.Stind_R4);
				else if (type == typeof(double)) ilGenerator.Emit(OpCodes.Stind_R8);
				else if (type == typeof(sbyte) || type == typeof(byte)) ilGenerator.Emit(OpCodes.Stind_I1);
				else if (type == typeof(short) || type == typeof(ushort)) ilGenerator.Emit(OpCodes.Stind_I2);
				else if (type == typeof(int) || type == typeof(uint)) ilGenerator.Emit(OpCodes.Stind_I4);
				else if (type == typeof(long) ||type == typeof(ulong)) ilGenerator.Emit(OpCodes.Stind_I8);
				else
					ilGenerator.Emit(OpCodes.Stobj, type);
			}
			else
				ilGenerator.Emit(OpCodes.Stind_Ref);
		}

		public static void EmitLoadIndirect(this ILGenerator ilGenerator, Type type)
		{
			if (type.IsEnum)
				type = Enum.GetUnderlyingType(type);

			if (type.IsValueType)
			{
				if (type == typeof(bool)) ilGenerator.Emit(OpCodes.Ldind_I1);
				else if (type == typeof(char)) ilGenerator.Emit(OpCodes.Ldind_I2);
				else if (type == typeof(float)) ilGenerator.Emit(OpCodes.Ldind_R4);
				else if (type == typeof(double)) ilGenerator.Emit(OpCodes.Ldind_R8);
				else if (type == typeof(byte)) ilGenerator.Emit(OpCodes.Ldind_U1);
				else if (type == typeof(sbyte)) ilGenerator.Emit(OpCodes.Ldind_I1);
				else if (type == typeof(ushort)) ilGenerator.Emit(OpCodes.Ldind_U2);
				else if (type == typeof(short)) ilGenerator.Emit(OpCodes.Ldind_I2);
				else if (type == typeof(int)) ilGenerator.Emit(OpCodes.Ldind_U4);
				else if (type == typeof(uint)) ilGenerator.Emit(OpCodes.Ldind_I4);
				else if (type == typeof(ulong)) ilGenerator.Emit(OpCodes.Ldind_I8);
				else if (type == typeof(long)) ilGenerator.Emit(OpCodes.Ldind_I8);
				else
					ilGenerator.Emit(OpCodes.Ldobj, type); 
			}
			else
				ilGenerator.Emit(OpCodes.Ldind_Ref);
		}

		
	}
}
