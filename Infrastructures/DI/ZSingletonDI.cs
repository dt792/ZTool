using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZTool.Infrastructures.DI;

public class ZSingletonDI
{
	public List<SingletonGetter> Getters = new List<SingletonGetter>();

	public string CheckHint = "";

	public ZSingletonDI()
	{
		Set(this);
	}

	public void Set(object obj)
	{
		Getters.Add(new SingletonGetter(obj.GetType(), obj.GetType(), this, obj));
	}

	public void Set<Target>() where Target : class, new()
	{
		Set(typeof(Target), typeof(Target));
	}

	public void Set<Actual, Target>() where Actual : class, Target, new()
	{
		Set(typeof(Actual), typeof(Target));
	}

	public void Set(Type type)
	{
		Set(type, type);
	}

	public void Set(Type targetType, Type type)
	{
		Getters.Add(new SingletonGetter(type, targetType, this));
	}

	public Target QuickGet<Target>() where Target : class, new()
	{
		Set<Target>();
		Check();
		return (Target)Get(typeof(Target));
	}

	public Target Get<Target>()
	{
		return (Target)Get(typeof(Target));
	}

	public object Get(Type targetType)
	{
		List<SingletonGetter> list = Getters.ToList();
		list.Reverse();
		SingletonGetter singletonGetter = list.FirstOrDefault((SingletonGetter g) => g.TargetType == targetType) ?? throw new InvalidOperationException($"{targetType} 未在容器中注册，请先 Set 或执行 Check");
		return singletonGetter.Get();
	}

	public List<Target> GetAll<Target>()
	{
		List<Target> list = new List<Target>();
		foreach (SingletonGetter item in from g in Getters.ToList()
			where g.TargetType == typeof(Target)
			select g)
		{
			list.Add((Target)item.Get());
		}
		return list;
	}

	public List<object> GetAll(Type type)
	{
		List<object> list = new List<object>();
		foreach (SingletonGetter item in Getters.Where((SingletonGetter g) => g.TargetType == type))
		{
			list.Add(item.Get());
		}
		return list;
	}

	public bool Check()
	{
		CheckHint = "";
		bool isOK = true;
		List<Type> checkedTypes = new List<Type>();
		SingletonGetter[] array = Getters.ToArray();
		foreach (SingletonGetter singletonGetter in array)
		{
			TrackOne(singletonGetter.ActualType);
		}
		return isOK;
		void TrackOne(Type requiredType)
		{
			checkedTypes.Add(requiredType);
			if (requiredType.IsGenericType && requiredType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return;
			}
			if (!Getters.Exists((SingletonGetter g) => g.TargetType == requiredType))
			{
				if (requiredType.IsAbstract)
				{
					CheckHint += $"{requiredType}是抽象类，但没有给定具体类型\n";
					isOK = false;
				}
				else
				{
					try
					{
						requiredType.GetConstructors().First((ConstructorInfo ctor) => ctor.GetParameters().Length == 0);
						Set(requiredType);
						CheckHint += $"已自动添加{requiredType}\n";
					}
					catch (Exception)
					{
						CheckHint += $"{requiredType}是没有无参的构造函数\n";
						isOK = false;
					}
				}
			}
			foreach (MemberInfo item in requiredType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>().Concat(requiredType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)))
			{
				DIAttribute customAttribute = item.GetCustomAttribute<DIAttribute>();
				if (customAttribute != null && !(customAttribute.ActualType != null))
				{
					Type type = ((item is PropertyInfo propertyInfo) ? propertyInfo.PropertyType : ((FieldInfo)item).FieldType);
					if (!checkedTypes.Contains(type))
					{
						TrackOne(type);
					}
				}
			}
		}
	}
}
