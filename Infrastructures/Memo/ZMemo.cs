using System.Collections.Generic;
using System.Reflection;
using ZTool.Bases;

namespace ZTool.Infrastructures.Memo;

public static class ZMemo
{
	private static readonly Dictionary<object, Dictionary<string, object>> saveDict = new Dictionary<object, Dictionary<string, object>>();

	public static void SaveMemo<T>(this T obj, string name = "zlast") where T : new()
    {
		if (!saveDict.TryGetValue(obj, out Dictionary<string, object> value))
		{
			value = new Dictionary<string, object>();
			saveDict.Add(obj, value);
		}
		value[name] = obj.DeepClone();
	}

	public static void LoadMemo<T>(this T obj, string name = "zlast") where T : notnull
	{
		if (saveDict.TryGetValue(obj, out Dictionary<string, object> value) && value.TryGetValue(name, out var value2))
		{
			Pump(value2, obj);
		}
	}

	private static void Pump(object from, object to)
	{
		PropertyInfo[] properties = from.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.SetMethod != null)
			{
				propertyInfo.SetValue(to, propertyInfo.GetValue(from));
			}
		}
		FieldInfo[] fields = from.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(to, fieldInfo.GetValue(from));
		}
	}
}
