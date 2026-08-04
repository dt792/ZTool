using System;
using System.Collections.Generic;

namespace ZTool.Infrastructures.Event;

public static class ZEventBus
{
	public static Dictionary<Type, object> EventDict = new Dictionary<Type, object>();

	public static E Let<E>() where E : new()
	{
		if (!EventDict.TryGetValue(typeof(E), out object value))
		{
			value = new E();
			EventDict.Add(typeof(E), value);
		}
		return (E)value;
	}

	public static void Publish<TEventType>() where TEventType : ZEvent, new()
	{
		Let<TEventType>().Publish();
	}

	public static void Subscribe<TEventType>(Action action) where TEventType : ZEvent, new()
	{
		Let<TEventType>().Subscribe(action);
	}
}
