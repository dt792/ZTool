using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.Infrastructures.Mapper;

public class Mapper<F, T> where T : new()
{
	private readonly List<MemberInfo> ignoreMembers = new List<MemberInfo>();

	private readonly Dictionary<MemberInfo, MemberInfo> mapDict = new Dictionary<MemberInfo, MemberInfo>();

	private Action<F, T>? finalAction;

	public Mapper()
	{
		FindMapMember();
	}

	private static (MemberInfo, Type) ParseExpr<S, M>(Expression<Func<S, M>> expr)
	{
		Expression body = expr.Body;
		if (1 == 0)
		{
		}
		MemberExpression memberExpression3;
		if (!(body is MemberExpression memberExpression))
		{
			if (body is UnaryExpression unaryExpression)
			{
				Expression operand = unaryExpression.Operand;
				if (operand is MemberExpression memberExpression2)
				{
					memberExpression3 = memberExpression2;
					goto IL_004b;
				}
			}
			memberExpression3 = null;
		}
		else
		{
			memberExpression3 = memberExpression;
		}
		goto IL_004b;
		IL_004b:
		if (1 == 0)
		{
		}
		MemberInfo memberInfo = memberExpression3?.Member;
		if (1 == 0)
		{
		}
		(MemberInfo, Type) result;
		if (!(memberInfo is PropertyInfo propertyInfo))
		{
			if (!(memberInfo is FieldInfo fieldInfo))
			{
				throw new InvalidOperationException($"{expr}不是合法的成员表达式");
			}
			result = (fieldInfo, fieldInfo.FieldType);
		}
		else
		{
			result = (propertyInfo, propertyInfo.PropertyType);
		}
		if (1 == 0)
		{
		}
		return result;
	}

	public Mapper<F, T> IgnoreProp(Expression<Func<F, object>> fromExpr)
	{
		ignoreMembers.Add(ParseExpr(fromExpr).Item1);
		return this;
	}

	public Mapper<F, T> MapMember(Expression<Func<F, object>> fromExpr, Expression<Func<T, object>> toExpr)
	{
		mapDict[ParseExpr(fromExpr).Item1] = ParseExpr(toExpr).Item1;
		return this;
	}

	public Mapper<F, T> Final(Action<F, T> finalAction)
	{
		this.finalAction = finalAction;
		return this;
	}

	public void FindMapMember()
	{
		Dictionary<string, MemberInfo> dictionary = CollectMembers(typeof(F));
		Dictionary<string, MemberInfo> dictionary2 = CollectMembers(typeof(T));
		foreach (KeyValuePair<string, MemberInfo> item in dictionary)
		{
			if (dictionary2.TryGetValue(item.Key, out var value))
			{
				mapDict[item.Value] = value;
			}
		}
	}

	private static Dictionary<string, MemberInfo> CollectMembers(Type type)
	{
		Dictionary<string, MemberInfo> dictionary = new Dictionary<string, MemberInfo>();
		PropertyInfo[] properties = type.GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			dictionary.TryAdd(Normalize(propertyInfo.Name), propertyInfo);
		}
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			dictionary.TryAdd(Normalize(fieldInfo.Name), fieldInfo);
		}
		return dictionary;
		static string Normalize(string name)
		{
			return name.Replace("_", "").ToLower();
		}
	}

	public object? SimpleDefaultMap(object? fromObj, Type toType)
	{
		if (fromObj == null)
		{
			return null;
		}
		if (fromObj.GetType() == toType)
		{
			return fromObj;
		}
		if (toType == typeof(string))
		{
			return fromObj.ToString();
		}
		if (toType.IsEnum)
		{
			return Enum.Parse(toType, fromObj.ToString());
		}
		if (toType == typeof(int))
		{
			return int.Parse(fromObj.ToString());
		}
		return null;
	}

	public void Map(F f, T t)
	{
		foreach (KeyValuePair<MemberInfo, MemberInfo> item in mapDict)
		{
			if (ignoreMembers.Contains(item.Key))
			{
				continue;
			}
			MemberInfo key = item.Key;
			if (1 == 0)
			{
			}
			object obj = ((key is PropertyInfo propertyInfo) ? propertyInfo.GetValue(f) : ((!(key is FieldInfo fieldInfo)) ? null : fieldInfo.GetValue(f)));
			if (1 == 0)
			{
			}
			object fromObj = obj;
			MemberInfo value = item.Value;
			MemberInfo memberInfo = value;
			if (!(memberInfo is PropertyInfo propertyInfo2))
			{
				if (memberInfo is FieldInfo fieldInfo2)
				{
					fromObj = SimpleDefaultMap(fromObj, fieldInfo2.FieldType);
					if (fromObj != null)
					{
						fieldInfo2.SetValue(t, fromObj);
					}
				}
			}
			else
			{
				fromObj = SimpleDefaultMap(fromObj, propertyInfo2.PropertyType);
				if (fromObj != null)
				{
					propertyInfo2.SetValue(t, fromObj);
				}
			}
		}
		finalAction?.Invoke(f, t);
	}

	public T Map(F f)
	{
		T val = new T();
		Map(f, val);
		return val;
	}
}
