namespace ZTool.Bases;
public static class FuncTool
{
    public static Action Pack<T1>(this Action<T1> action, T1 arg0)
    {
        return () => { action(arg0); };
    }
    public static Action Pack<T1, T2>(this Action<T1, T2> action, T1 arg0, T2 arg1)
    {
        return () => { action(arg0, arg1); };
    }
    public static Action Pack<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg0, T2 arg1, T3 arg2)
    {
        return () => { action(arg0, arg1, arg2); };
    }
    public static Action Pack<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3)
    {
        return () => { action(arg0, arg1, arg2, arg3); };
    }
    public static Action Pack<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
    {
        return () => { action(arg0, arg1, arg2, arg3, arg4); };
    }
    public static Action Pack<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4, T6 arg5)
    {
        return () => { action(arg0, arg1, arg2, arg3, arg4, arg5); };
    }

    public static Func<R> Pack<T1, R>(this Func<T1, R> func, T1 arg0)
    {
        return () => { return func(arg0); };
    }
    public static Func<R> Pack<T1, T2, R>(this Func<T1, T2, R> func, T1 arg0, T2 arg1)
    {
        return () => { return func(arg0, arg1); };
    }
    public static Func<R> Pack<T1, T2, T3, R>(this Func<T1, T2, T3, R> func, T1 arg0, T2 arg1, T3 arg2)
    {
        return () => { return func(arg0, arg1, arg2); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3)
    {
        return () => { return func(arg0, arg1, arg2, arg3); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
    {
        return () => { return func(arg0, arg1, arg2, arg3, arg4); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4, T6 arg5)
    {
        return () => { return func(arg0, arg1, arg2, arg3, arg4, arg5); };
    }
}
