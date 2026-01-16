namespace ZTool.Bases;
public static class EnumTool
{
    public static List<T> GetEnumList<T>() where T : Enum
    {
        List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
        return list;
    }
}
