using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZTool.Bases; // 记得加上你的命名空间

public static class JsonTool
{
    // 1. 改名为 ToJson，并增加一个布尔参数控制是否缩进
    public static string ToJson(this object obj, bool indented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented // 控制缩进
        };
        return JsonSerializer.Serialize(obj, options);
    }

    // 2. 改名为 ToObj
    public static T? ToObj<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    // 保留你原来的带引用保留的方法
    public static string RefSerialize(this object obj)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        return JsonSerializer.Serialize(obj, options);
    }

    public static T? RefDeserialize<T>(this string json)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        return JsonSerializer.Deserialize<T>(json, options);
    }
}