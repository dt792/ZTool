using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTools.Infrastructures;

public abstract class Event
{
    public List<Action> subs = new();
    public void Publish() => subs.ForEach(a => a.Invoke());
    public void Subscribe(Action action) => subs.Add(action);
    public void UnSubscribe(Action action) => subs.Remove(action);
}
public abstract class Event<T>
{
    public List<Action<T>> subs = new();
    public void Publish(T t) => subs.ForEach(a => a.Invoke(t));
    public void Subscribe(Action<T> action) => subs.Add(action);
    public void UnSubscribe(Action<T> action) => subs.Remove(action);
}
public abstract class Event<T1, T2>
{
    public List<Action<T1, T2>> subs = new();
    public void Publish(T1 t1, T2 t2) => subs.ForEach(a => a.Invoke(t1, t2));
    public void Subscribe(Action<T1, T2> action) => subs.Add(action);
    public void UnSubscribe(Action<T1, T2> action) => subs.Remove(action);
}
public abstract class Event<T1, T2, T3>
{
    public List<Action<T1, T2, T3>> subs = new();
    public void Publish(T1 t1, T2 t2, T3 t3) => subs.ForEach(a => a.Invoke(t1, t2, t3));
    public void Subscribe(Action<T1, T2, T3> action) => subs.Add(action);
    public void UnSubscribe(Action<T1, T2, T3> action) => subs.Remove(action);
}

public static class ZEvent
{
    public static Dictionary<Type, object> EventDict = new();
    public static E Let<E>() where E : new()
    {
        if (!EventDict.ContainsKey(typeof(E)))
            EventDict.Add(typeof(E), new E());
        return (E)EventDict[typeof(E)];
    }
    public static void Publish<TEventType>() where TEventType : Event, new()
    {
        Let<TEventType>().Publish();
    }
    public static void Subscribe<TEventType>(Action action) where TEventType : Event, new()
    {
        Let<TEventType>().Subscribe(action);
    }
}