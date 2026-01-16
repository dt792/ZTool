using System.Reflection;

namespace ZTool.Structures;
public abstract class RichEnum<TEnum, Content> : IEquatable<RichEnum<TEnum, Content>>
    where TEnum : RichEnum<TEnum, Content>
{
    public Content Value { get; protected init; }
    public string Name { get; protected init; } = string.Empty;
    protected RichEnum(string name, Content value)
    {
        Value = value;
        Name = name;
    }

    /// <summary>
    /// 获取具体枚举
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TEnum? FromName(string name)
    {
        return enumerationsDict.TryGetValue(name, out TEnum? e) ? e : default;
    }
    /// <summary>
    /// 获取所有枚举
    /// </summary>
    public static readonly IEnumerable<TEnum> Enumerations = CreateEnumerations().Select(pair => pair.Value);


    public bool Equals(RichEnum<TEnum, Content>? other)
    {
        if (other is null)
        {
            return false;
        }
        return GetType() == other.GetType() && other.Value.Equals(Value);
    }
    public override bool Equals(object? obj)
    {
        return obj is RichEnum<TEnum, Content> other && Equals(other);
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    public override string ToString()
    {
        return Name;
    }

    #region 从父类获取子类的枚举量
    private static readonly Dictionary<string, TEnum> enumerationsDict = CreateEnumerations();
    private static Dictionary<string, TEnum> CreateEnumerations()
    {
        var type = typeof(TEnum);
        var fieldsForType = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => type.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);
        var d = fieldsForType.ToDictionary(x => x.Name, x => x);
        return fieldsForType.ToDictionary(x => x.Name, x => x);
    }
    #endregion
}