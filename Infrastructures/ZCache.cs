namespace ZTool.Infrastructures;
/// <summary>
/// 当一个函数的计算量比较大时 且 结果只与参数相关 时可以使用
/// CacheServer将记录调用的参数并在下一次遇到同样的参数时直接返回上一次的结果
/// PS 部分参数可能需要重写Equals方法
/// 使用方法
/// 使用 要缓存的计算函数 构造 CacheServer对象
/// 调用 .Call并传入参数 当作正常参数使用
/// </summary>
public abstract class ZCacherBase
{
    public int MaxCacheNum { get; set; }
}
public class ZCacher<T, R> : ZCacherBase
{
    Dictionary<T, R> HistoryResults = new Dictionary<T, R>();
    // 索引器
    public R this[T index]
    {
        get => HistoryResults[index];
        set => HistoryResults[index] = value;
    }
}
/// <summary>
/// 基础类型需要最好写成int?
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="R"></typeparam>
public class ZCacher<T1, T2, R> : ZCacherBase
{
    struct T<T1, T2>
    {
        public T1 A1 { get; set; }
        public T2 A2 { get; set; }
    }
    Dictionary<T<T1, T2>, R> HistoryResults = new();
    // 索引器
    public R this[T1 index1, T2 index2]
    {
        get => HistoryResults[new T<T1, T2>() { A1 = index1, A2 = index2 }];
        set => HistoryResults[new T<T1, T2>() { A1 = index1, A2 = index2 }] = value;
    }
}
public class ZCacher<T1, T2, T3, R> : ZCacherBase
{
    struct T<T1, T2, T3>
    {
        public T1 A1 { get; set; }
        public T2 A2 { get; set; }
        public T3 A3 { get; set; }
    }
    Dictionary<T<T1, T2, T3>, R> HistoryResults = new();
    // 索引器
    public R this[T1 index1, T2 index2, T3 index3]
    {
        get => HistoryResults[new T<T1, T2, T3>() { A1 = index1, A2 = index2, A3 = index3 }];
        set => HistoryResults[new T<T1, T2, T3>() { A1 = index1, A2 = index2, A3 = index3 }] = value;
    }
}