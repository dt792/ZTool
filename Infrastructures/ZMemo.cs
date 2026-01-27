using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ZTool.Bases;

namespace ZTool.Infrastructures;

public static class ZMemo
{
    public static void SaveMemo<T>(this T obj, string name = "zlast") where T : new()
    {
        if (!saveDict.ContainsKey(obj))
        {
            saveDict.Add(obj, []);
        }
        if (saveDict[obj].ContainsKey(name))
        {
            saveDict[obj][name] = obj.DeepClone();
        }
        else
        {
            saveDict[obj].Add(name, obj.DeepClone());
        }
    }
    public static void LoadMemo<T>(this T obj, string name = "zlast") where T : new()
    {
        pump(saveDict[obj][name], obj);
    }
    static Dictionary<object, Dictionary<string, object>> saveDict = [];
    static void pump(object from, object to)
    {
        foreach (var prop in from.GetType().GetProperties())
        {
            if (prop.SetMethod is not null)
                prop.SetValue(to, prop.GetValue(from));
        }
        foreach (var field in from.GetType().GetFields())
        {
            field.SetValue(to, field.GetValue(from));
        }
    }
}
