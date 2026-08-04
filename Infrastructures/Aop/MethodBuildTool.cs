using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ZTool.Infrastructures.Aop;

public static class MethodBuildTool
{
	public static Action Pack(Action<InvocationContext> action, InvocationContext obj)
	{
		return delegate
		{
			action(obj);
		};
	}

	public static T Create<T>() where T : new()
	{
		return new T();
	}

	public static void ConstructMethod(TypeBuilder tb, ILGenerator overrideIL, List<Attribute> attris, MethodInfo orgMethod)
	{
		bool flag = orgMethod.ReturnType != typeof(void);
		List<FieldInfo> list = new List<FieldInfo>();
		for (int i = 0; i < attris.Count; i++)
		{
			list.Add(tb.DefineField($"{orgMethod.Name}_f_{i}_周启新", attris[i].GetType(), FieldAttributes.Private));
		}
		MethodBuilder methodBuilder = tb.DefineMethod(orgMethod.Name + "_base_周启新", MethodAttributes.Private, typeof(void), new Type[1] { typeof(InvocationContext) });
		ILGenerator iLGenerator = methodBuilder.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		int num = 0;
		ParameterInfo[] parameters = orgMethod.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Ldfld, typeof(InvocationContext).GetField("Parameters"));
			iLGenerator.Emit(OpCodes.Ldc_I4, num++);
			iLGenerator.Emit(OpCodes.Callvirt, typeof(object[]).GetMethod("GetValue", new Type[1] { typeof(int) }));
			EmitConvertFromObject(iLGenerator, parameterInfo.ParameterType);
		}
		iLGenerator.Emit(OpCodes.Call, orgMethod);
		if (flag)
		{
			LocalBuilder local = iLGenerator.DeclareLocal(typeof(object));
			if (orgMethod.ReturnType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Box, orgMethod.ReturnType);
			}
			iLGenerator.Emit(OpCodes.Stloc, local);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Stfld, typeof(InvocationContext).GetField("ReturnValue"));
		}
		iLGenerator.Emit(OpCodes.Ret);
		List<MethodInfo> list2 = new List<MethodInfo>();
		for (int k = 0; k < attris.Count; k++)
		{
			MethodBuilder methodBuilder2 = tb.DefineMethod($"{orgMethod.Name}_{k}_周启新", MethodAttributes.Private, typeof(void), new Type[1] { typeof(InvocationContext) });
			ILGenerator iLGenerator2 = methodBuilder2.GetILGenerator();
			iLGenerator2.Emit(OpCodes.Ldarg_0);
			iLGenerator2.Emit(OpCodes.Ldfld, list[k]);
			iLGenerator2.Emit(OpCodes.Ldarg_1);
			iLGenerator2.Emit(OpCodes.Callvirt, attris[k].GetType().GetMethod("Invoke"));
			iLGenerator2.Emit(OpCodes.Ret);
			list2.Add(methodBuilder2);
		}
		MethodBuilder methodBuilder3 = tb.DefineMethod(orgMethod.Name + "_init_周启新", MethodAttributes.Private, typeof(void), new Type[1] { typeof(InvocationContext) });
		ILGenerator iLGenerator3 = methodBuilder3.GetILGenerator();
		for (int l = 0; l < attris.Count; l++)
		{
			Label label = iLGenerator3.DefineLabel();
			iLGenerator3.Emit(OpCodes.Ldarg_0);
			iLGenerator3.Emit(OpCodes.Ldfld, list[l]);
			iLGenerator3.Emit(OpCodes.Ldnull);
			iLGenerator3.Emit(OpCodes.Ceq);
			iLGenerator3.Emit(OpCodes.Brfalse_S, label);
			iLGenerator3.Emit(OpCodes.Ldarg_0);
			iLGenerator3.Emit(OpCodes.Call, typeof(MethodBuildTool).GetMethod("Create").MakeGenericMethod(attris[l].GetType()));
			iLGenerator3.Emit(OpCodes.Stfld, list[l]);
			iLGenerator3.MarkLabel(label);
		}
		for (int m = 0; m < list.Count; m++)
		{
			iLGenerator3.Emit(OpCodes.Ldarg_0);
			iLGenerator3.Emit(OpCodes.Ldfld, list[m]);
			iLGenerator3.Emit(OpCodes.Ldarg_0);
			iLGenerator3.Emit(OpCodes.Ldftn, (m + 1 < list2.Count) ? list2[m + 1] : methodBuilder);
			iLGenerator3.Emit(OpCodes.Newobj, typeof(Action<InvocationContext>).GetConstructors()[0]);
			iLGenerator3.Emit(OpCodes.Ldarg_1);
			iLGenerator3.Emit(OpCodes.Call, typeof(MethodBuildTool).GetMethod("Pack"));
			iLGenerator3.Emit(OpCodes.Stfld, typeof(InvokerAttribute).GetField("Next"));
		}
		iLGenerator3.Emit(OpCodes.Ret);
		overrideIL.DeclareLocal(typeof(InvocationContext));
		overrideIL.DeclareLocal(typeof(List<object>));
		overrideIL.Emit(OpCodes.Newobj, typeof(InvocationContext).GetConstructors()[0]);
		overrideIL.Emit(OpCodes.Stloc_0);
		overrideIL.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructors()[0]);
		overrideIL.Emit(OpCodes.Stloc_1);
		int num2 = 1;
		ParameterInfo[] parameters2 = orgMethod.GetParameters();
		foreach (ParameterInfo parameterInfo2 in parameters2)
		{
			overrideIL.Emit(OpCodes.Ldloc_1);
			overrideIL.Emit(OpCodes.Ldarg_S, num2++);
			if (parameterInfo2.ParameterType.IsValueType)
			{
				overrideIL.Emit(OpCodes.Box, parameterInfo2.ParameterType);
			}
			overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod("Add"));
		}
		overrideIL.Emit(OpCodes.Ldloc_0);
		overrideIL.Emit(OpCodes.Ldloc_1);
		overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod("ToArray"));
		overrideIL.Emit(OpCodes.Stfld, typeof(InvocationContext).GetField("Parameters"));
		overrideIL.Emit(OpCodes.Ldarg_0);
		overrideIL.Emit(OpCodes.Ldloc_0);
		overrideIL.Emit(OpCodes.Call, methodBuilder3);
		overrideIL.Emit(OpCodes.Ldarg_0);
		overrideIL.Emit(OpCodes.Ldloc_0);
		overrideIL.Emit(OpCodes.Call, list2[0]);
		if (flag)
		{
			overrideIL.Emit(OpCodes.Ldloc_0);
			overrideIL.Emit(OpCodes.Ldfld, typeof(InvocationContext).GetField("ReturnValue"));
			EmitConvertFromObject(overrideIL, orgMethod.ReturnType);
		}
		overrideIL.Emit(OpCodes.Ret);
	}

	private static void EmitConvertFromObject(ILGenerator il, Type targetType)
	{
		if (targetType.IsValueType)
		{
			il.Emit(OpCodes.Unbox_Any, targetType);
		}
		else
		{
			il.Emit(OpCodes.Castclass, targetType);
		}
	}
}
