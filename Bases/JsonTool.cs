using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZTool.Bases;

public class JsonTool
{
    public string RefSerialize(object obj)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(obj, options);
        return json;
    }
    public T RefDeserialize<T>(string json)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
        T obj = JsonSerializer.Deserialize<T>(json, options);
        return obj;
    }
    public string Serialize(object obj)
    {
        string json = JsonSerializer.Serialize(obj);
        return json;
    }
    public T Deserialize<T>(string json)
    {
        T obj = JsonSerializer.Deserialize<T>(json);
        return obj;
    }
}
