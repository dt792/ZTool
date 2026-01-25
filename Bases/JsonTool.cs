using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZTool.Bases;

public static class JsonTool
{
    public static string RefSerialize(object obj)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(obj, options);
        return json;
    }
    public static T RefDeserialize<T>(string json)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        T obj = JsonSerializer.Deserialize<T>(json, options);
        return obj;
    }
    public static string Serialize(object obj)
    {
        string json = JsonSerializer.Serialize(obj);
        return json;
    }
    public static T Deserialize<T>(string json)
    {
        T obj = JsonSerializer.Deserialize<T>(json);
        return obj;
    }
}
