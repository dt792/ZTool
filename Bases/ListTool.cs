namespace ZTool.Bases;
public static class ListTool
{
    public static IEnumerable<(int index, T item)> Indexlize<T>(this IEnumerable<T> enumerable)
    {
        int i = 0;
        foreach (var item in enumerable)
        {
            yield return (i++, item);
        }
    }
    /// <summary>
    /// 获取对象在队列中的前后对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static (T?, T?) FinePNObj<T>(List<T> values, T obj)
    {
        int index = values.IndexOf(obj);
        T p = default;
        T n = default;
        if (index > 0)
            p = values[index - 1];
        if (index < values.Count - 1)
            n = values[index + 1];
        return (p, n);
    }
    
}
