namespace ZTool.Structures;

/// <summary>
/// 享元：持有共享（内在）状态，独有（外在）状态在使用时由调用方传入。
/// </summary>
/// <typeparam name="TShared">共享状态类型</typeparam>
public class ZFlyweight<TShared>
{
    public TShared SharedState { get; }

    public ZFlyweight(TShared sharedState)
    {
        SharedState = sharedState;
    }
}

/// <summary>
/// 享元工厂：创建并管理享元对象，确保相同共享状态只存在一个实例。
/// 客户端请求享元时，工厂返回已有实例或创建新实例。
/// </summary>
/// <typeparam name="TShared">共享状态类型</typeparam>
public class ZFlyweightFactory<TShared>
{
    private readonly Dictionary<string, ZFlyweight<TShared>> flyweights = new();
    private readonly Func<TShared, string> keySelector;

    /// <summary>
    /// 当前管理的享元数量
    /// </summary>
    public int Count => flyweights.Count;

    /// <summary>
    /// 当前所有享元的键
    /// </summary>
    public IReadOnlyCollection<string> Keys => flyweights.Keys;

    /// <param name="keySelector">从共享状态计算唯一键的方法</param>
    /// <param name="initStates">初始共享状态</param>
    public ZFlyweightFactory(Func<TShared, string> keySelector, params TShared[] initStates)
    {
        this.keySelector = keySelector;
        foreach (var state in initStates)
        {
            flyweights[keySelector(state)] = new ZFlyweight<TShared>(state);
        }
    }

    /// <summary>
    /// 获取具有给定共享状态的享元，不存在时创建新实例
    /// </summary>
    public ZFlyweight<TShared> GetFlyweight(TShared sharedState)
    {
        string key = keySelector(sharedState);
        if (!flyweights.TryGetValue(key, out var flyweight))
        {
            flyweight = new ZFlyweight<TShared>(sharedState);
            flyweights.Add(key, flyweight);
        }
        return flyweight;
    }
}
