using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZTool.Infrastructures.Alias;

public static class ZAlias
{
	public static Dictionary<string, string[]> GetAlias(IEnumerable<Assembly> assemblies, Func<Type, bool>? filter = null)
	{
		if (filter == null)
		{
			filter = (Type _) => true;
		}
		return (from type in assemblies.SelectMany((Assembly assembly) => assembly.GetTypes())
			where type.GetCustomAttribute<AliasAttribute>() != null && filter(type)
			select type).ToDictionary((Type type) => type.Name, (Type type) => type.GetCustomAttribute<AliasAttribute>().Alias);
	}

	public static Dictionary<string, string[]> GetAlias(IEnumerable<Assembly> assemblies, Func<Type, List<MemberInfo>> mapper, Func<MemberInfo, bool>? filter = null)
	{
		if (filter == null)
		{
			filter = (MemberInfo _) => true;
		}
		return (from member in assemblies.SelectMany((Assembly assembly) => assembly.GetTypes()).SelectMany((Type type) => mapper(type))
			where member.GetCustomAttribute<AliasAttribute>() != null && filter(member)
			select member).ToDictionary((MemberInfo member) => member.Name, (MemberInfo member) => member.GetCustomAttribute<AliasAttribute>().Alias);
	}

	public static string[] GetAlias<T>()
	{
		return typeof(T).GetAlias();
	}

	public static string[] GetAlias(this Type type)
	{
		return type.GetCustomAttribute<AliasAttribute>()?.Alias ?? Array.Empty<string>();
	}
}
