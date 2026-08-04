using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ZTool.Infrastructures.Aop;

public static class ZClass
{
	private static TypeBuilder CreateTypeBuilder(Type type)
	{
		AssemblyName assemblyName = new AssemblyName($"{type}OverrideAssembly");
		return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name + ".dll").DefineType(type.Name + "OverrideType", type.Attributes, type);
	}

	private static Dictionary<MethodInfo, List<Attribute>> FindOverrideMethods(Type enrichType)
	{
		Dictionary<MethodInfo, List<Attribute>> dictionary = new Dictionary<MethodInfo, List<Attribute>>();
		MethodInfo[] methods = enrichType.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsVirtual)
			{
				List<Attribute> list = methodInfo.GetCustomAttributes().OfType<InvokerAttribute>().Cast<Attribute>()
					.ToList();
				if (list.Count > 0)
				{
					dictionary.Add(methodInfo, list);
				}
			}
		}
		return dictionary;
	}

	private static void OverrideMethods(TypeBuilder tb, Dictionary<MethodInfo, List<Attribute>> methodAttris)
	{
		foreach (KeyValuePair<MethodInfo, List<Attribute>> methodAttri in methodAttris)
		{
			MethodInfo key = methodAttri.Key;
			MethodBuilder methodBuilder = tb.DefineMethod(key.Name, key.Attributes, key.ReturnType, (from p in key.GetParameters()
				select p.ParameterType).ToArray());
			tb.DefineMethodOverride(methodBuilder, key);
			MethodBuildTool.ConstructMethod(tb, methodBuilder.GetILGenerator(), methodAttri.Value, key);
		}
	}

	public static Type Make<T>()
	{
		TypeBuilder typeBuilder = CreateTypeBuilder(typeof(T));
		OverrideMethods(typeBuilder, FindOverrideMethods(typeof(T)));
		return typeBuilder.CreateType();
	}
}
