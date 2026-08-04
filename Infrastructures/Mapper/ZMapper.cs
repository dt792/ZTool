using System;
using System.Collections.Generic;

namespace ZTool.Infrastructures.Mapper;

public static class ZMapper
{
	public static Dictionary<(Type from, Type to), object> Mappers { get; set; } = new Dictionary<(Type, Type), object>();

	public static T To<T>(this object obj) where T : new()
	{
		if (!Mappers.TryGetValue((obj.GetType(), typeof(T)), out object value))
		{
			value = Define(obj.GetType(), typeof(T));
		}
		dynamic val = value;
		return (T)val.Map((dynamic)obj);
	}

	public static Mapper<From, To> Define<From, To>() where To : new()
	{
		Mapper<From, To> mapper = new Mapper<From, To>();
		Mappers[(typeof(From), typeof(To))] = mapper;
		return mapper;
	}

	public static object Define(Type from, Type to)
	{
		object obj = Activator.CreateInstance(typeof(Mapper<, >).MakeGenericType(from, to));
		Mappers[(from, to)] = obj;
		return obj;
	}

	public static To Map<From, To>(From from) where To : new()
	{
		if (!Mappers.TryGetValue((typeof(From), typeof(To)), out object value))
		{
			value = Define<From, To>();
		}
		return ((Mapper<From, To>)value).Map(from);
	}
}
