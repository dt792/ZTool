using System;
using System.Collections.Generic;

namespace ZTool.Infrastructures.Event;

public abstract class ZEvent
{
	protected List<Action> subs = new List<Action>();

	public void Publish()
	{
		subs.ForEach(delegate(Action a)
		{
			a();
		});
	}

	public void Subscribe(Action action)
	{
		subs.Add(action);
	}

	public void UnSubscribe(Action action)
	{
		subs.Remove(action);
	}
}
public abstract class ZEvent<T>
{
	protected List<Action<T>> subs = new List<Action<T>>();

	public void Publish(T t)
	{
		subs.ForEach(delegate(Action<T> a)
		{
			a(t);
		});
	}

	public void Subscribe(Action<T> action)
	{
		subs.Add(action);
	}

	public void UnSubscribe(Action<T> action)
	{
		subs.Remove(action);
	}
}
public abstract class ZEvent<T1, T2>
{
	protected List<Action<T1, T2>> subs = new List<Action<T1, T2>>();

	public void Publish(T1 t1, T2 t2)
	{
		subs.ForEach(delegate(Action<T1, T2> a)
		{
			a(t1, t2);
		});
	}

	public void Subscribe(Action<T1, T2> action)
	{
		subs.Add(action);
	}

	public void UnSubscribe(Action<T1, T2> action)
	{
		subs.Remove(action);
	}
}
public abstract class ZEvent<T1, T2, T3>
{
	protected List<Action<T1, T2, T3>> subs = new List<Action<T1, T2, T3>>();

	public void Publish(T1 t1, T2 t2, T3 t3)
	{
		subs.ForEach(delegate(Action<T1, T2, T3> a)
		{
			a(t1, t2, t3);
		});
	}

	public void Subscribe(Action<T1, T2, T3> action)
	{
		subs.Add(action);
	}

	public void UnSubscribe(Action<T1, T2, T3> action)
	{
		subs.Remove(action);
	}
}
