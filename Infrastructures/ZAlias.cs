using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ZTool.Infrastructures;
/// <summary>
/// 别名 
/// 使用:[Alias("别名1","别名2",...)]
/// </summary>
public class AliasAttribute : Attribute
{
    public string[] Ailas { get; set; }
    public AliasAttribute(params string[] chineseName)
    {
        Ailas = chineseName;
    }
}

public static class ZAilas
{
    /// <summary>
    /// 从程序集中搜索所有类,如果其带有中文名标签则添加到词典中
    /// </summary>
    /// <param name="assemblies">待搜索的程序集</param>
    /// <param name="filter">过滤条件</param>
    /// <returns>中文词典</returns>
    public static Dictionary<string, string[]> GetAlias(IEnumerable<Assembly> assemblies, Func<Type, bool>? filter = null)
    {
        if (filter is null)
            filter = type => true;
        var result = assemblies
            .SelectMany(assemble => assemble.GetTypes())
            .Where(type => type.GetCustomAttribute(typeof(AliasAttribute)) is not null)
            .Select(member => (member.Name, ((AliasAttribute)member.GetCustomAttribute(typeof(AliasAttribute))).Ailas))//keySelector: (a, b) => a,elementSelector:(a, b) => b
            .ToDictionary(x => x.Name, x => x.Ailas);
        return result;
    }

    /// <summary>
    /// 从程序集中搜索所有类,使用mapper指定搜索的成员信息并判断时候满足条件,如果其带有中文名标签则添加到词典中
    /// </summary>
    /// <param name="assemblies">待搜索的程序集</param>
    /// <param name="mapper">由用户指定从类型到所需成员的映射</param>
    /// <param name="filter">过滤条件</param>
    /// <returns>中文词典</returns>
    public static Dictionary<string, string[]> GetAlias(IEnumerable<Assembly> assemblies, Func<Type, List<MemberInfo>> mapper, Func<MemberInfo, bool>? filter = null)
    {
        if (filter is null)
            filter = member => true;
        Dictionary<string, List<string>> ChineseNames = new Dictionary<string, List<string>>();
        var result = assemblies
            .SelectMany(assemble => assemble.GetTypes())
            .SelectMany(type => mapper(type))
            .Where(member => member.GetCustomAttribute(typeof(AliasAttribute)) is not null)
            .Where(member => filter(member))
            .Select(member => (member.Name, ((AliasAttribute)member.GetCustomAttribute(typeof(AliasAttribute))).Ailas))
            .ToDictionary(x => x.Name, x => x.Ailas);
        return result;
    }
    /// <summary>
    /// 从程序集中搜索所有类,使用mapper指定搜索的成员信息并判断时候满足条件,如果其带有中文名标签则添加到词典中
    /// </summary>
    /// <param name="assemblies">待搜索的程序集</param>
    /// <param name="mapper">由用户指定从类型到所需成员的映射</param>
    /// <param name="filter">过滤条件</param>
    /// <returns>中文词典</returns>
    public static string[] GetAlias<T>()
    {
        var type = typeof(T);
        var attri = ((AliasAttribute)type.GetCustomAttribute(typeof(AliasAttribute)));
        if (attri is not null)
        {
            return attri.Ailas;
        }
        else
        {
            return [];
        }
    }
    public static string[] GetAlias(this Type type)
    {
        var attri = ((AliasAttribute)type.GetCustomAttribute(typeof(AliasAttribute)));
        if (attri is not null)
        {
            return attri.Ailas;
        }
        else
        {
            return [];
        }
    }
}