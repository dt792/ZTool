using System.Runtime.CompilerServices;

namespace ZTool.Sugers;

public static class ZSugers
{
    extension<T, Result>(T)
    {
        public static Result operator >>(T source, Func<T, Result> func)
            => func(source);
    }
    extension<In, Mid, Out>(Func<In, Mid>)
    {
        public static Func<In, Out> operator +(Func<In, Mid> func1, Func<Mid, Out> func2)
        {
            return (a) => func2(func1(a));
        }
    }
    public static TaskAwaiter GetAwaiter(this int waitMilliSecond)
    {
        return Task.Delay(waitMilliSecond).GetAwaiter();
    }
    public static IEnumerator<int> GetEnumerator(this int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return i;
        }
    }
}
