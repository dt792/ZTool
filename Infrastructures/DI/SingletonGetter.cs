using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZTool.Infrastructures.DI;

public class SingletonGetter
{
	private object? instance;

	public ZSingletonDI Container { get; init; }

	public Type ActualType { get; init; }

	public Type TargetType { get; init; }

	public SingletonGetter(Type actualType, Type targetType, ZSingletonDI container)
	{
		ActualType = actualType;
		TargetType = targetType;
		Container = container;
	}

	public SingletonGetter(Type actualType, Type targetType, ZSingletonDI container, object instance)
		: this(actualType, targetType, container)
	{
		this.instance = instance;
	}

	public object Get()
	{
		if (instance != null)
		{
			return instance;
		}
		ConstructorInfo constructorInfo = ActualType.GetConstructors().FirstOrDefault((ConstructorInfo ctor) => ctor.GetParameters().Length == 0) ?? throw new InvalidOperationException($"{ActualType} 没有无参构造函数，无法由容器创建");
		instance = constructorInfo.Invoke(null);
		InjectDI(instance);
		if (instance is ISingletonInit singletonInit)
		{
			singletonInit.Init();
		}
		return instance;
	}

	public void InjectDI(object obj)
	{
		Type type = obj.GetType();
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			DIAttribute customAttribute = propertyInfo.GetCustomAttribute<DIAttribute>();
			if (customAttribute != null)
			{
				propertyInfo.SetValue(obj, Resolve(propertyInfo.PropertyType, customAttribute));
			}
		}
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			DIAttribute customAttribute2 = fieldInfo.GetCustomAttribute<DIAttribute>();
			if (customAttribute2 != null)
			{
				fieldInfo.SetValue(obj, Resolve(fieldInfo.FieldType, customAttribute2));
			}
		}
	}

	private object Resolve(Type memberType, DIAttribute di)
	{
		if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(List<>))
		{
			Type type = memberType.GenericTypeArguments[0];
			IList list = (IList)Activator.CreateInstance(memberType);
			foreach (object item in Container.GetAll(type))
			{
				list.Add(item);
			}
			return list;
		}
		if (di.ActualType != null)
		{
			object obj = Activator.CreateInstance(di.ActualType);
			InjectDI(obj);
			if (obj is ISingletonInit singletonInit)
			{
				singletonInit.Init();
			}
			return obj;
		}
		return Container.Get(memberType);
	}

	public override string ToString()
	{
		return $"SingletonGetter:{ActualType}->{TargetType}";
	}
}
