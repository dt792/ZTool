using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace ZTool.Bases;
public static class ObjectTool
{
    public static string PrintProps(object obj)
    {
        StringBuilder stringBuilder = new StringBuilder();
        PropertyInfo[] properties = obj.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            stringBuilder.AppendLine($"{property.Name}:{property.GetValue(obj)}");
        }
        return stringBuilder.ToString();
    }
    public static T ShowllaClone<T>(this T obj) where T : new()
    {
        var newObj = (T)RuntimeHelpers.GetUninitializedObject(obj.GetType());
        foreach (var prop in typeof(T).GetProperties())
        {
            if (prop.SetMethod is not null)
                prop.SetValue(newObj, prop.GetValue(obj));
        }
        foreach (var field in typeof(T).GetFields())
        {
            field.SetValue(newObj, field.GetValue(obj));
        }
        return newObj;
    }
    public static T DeepClone<T>(this T obj) where T : new()
    {
        return (T)recursiveClone(obj);
    }
    static object recursiveClone(object obj)
    {
        if(obj.GetType().IsPrimitive)
        {
            return obj;
        }
        else if (obj is string)
        {
            return obj;
        }
        else if (obj.GetType().IsClass)
        {
            var newObj = RuntimeHelpers.GetUninitializedObject(obj.GetType());
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.SetMethod is not null)
                    prop.SetValue(newObj, recursiveClone(prop.GetValue(obj)));
            }
            foreach (var field in obj.GetType().GetFields())
            {
                field.SetValue(newObj, recursiveClone(field.GetValue(obj)));
            }
            return newObj;
        }
        else
        {
            var newObj = RuntimeHelpers.GetUninitializedObject(obj.GetType());
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.SetMethod is not null)
                    prop.SetValue(newObj, prop.GetValue(obj));
            }
            foreach (var field in obj.GetType().GetFields())
            {
                field.SetValue(newObj, field.GetValue(obj));
            }
            return newObj;
        }
    }

}
