using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;

namespace ZTool.Infrastructures;
public interface ISingletonInit
{
    public void Init();
}
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
/// <summary>
/// 用于Property或者Field,可绕过 private 访问控制 直接注入
/// </summary>
public class DIAttribute : Attribute
{
    public DIAttribute()
    {
    }
    //解析时使用特殊单例
    public DIAttribute(Type actualType)
    {
        ActualType = actualType;
    }
    public Type ActualType { get; set; }
}
//开发阶段在特性指定类型（更高优先级）
//发布阶段可以在Container指定具体类型
public class ZSingletonDI
{
    public ZSingletonDI()
    {
        Set(this);
    }
    public List<SignletonGetter> Getters = new List<SignletonGetter>();

    public void Set(object obj)
    {
        Getters.Add(new SignletonGetter(obj.GetType(), obj.GetType(), this, obj));
    }

    /// <summary>
    /// 注册单例
    /// </summary>
    /// <typeparam name="Actual"></typeparam>
    public void Set<Target>() where Target : class, new()
    {
        Set(typeof(Target), typeof(Target));
    }
    /// <summary>
    /// 注册单例
    /// </summary>
    /// <typeparam name="Actual"></typeparam>
    public void Set<Actual, Target>() where Actual : class, Target, new()
    {
        Set(typeof(Actual), typeof(Target));
    }
    /// <summary>
    /// 注册单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Set(Type type)
    {
        Set(type, type);
    }
    /// <summary>
    /// 注册单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Set(Type targetType, Type type)
    {
        Getters.Add(new SignletonGetter(type, targetType, this));
    }
    public Target QuickGet<Target>()where Target: class, new()
    {
        Set<Target>();
        Check();
        return (Target)Get(typeof(Target));
    }
    public Target Get<Target>()
    {
        return (Target)Get(typeof(Target));
    }
    /// <summary>
    /// 适用于需要在运行时才能确定的类型
    /// </summary>
    /// <typeparam name="T">用于最后强制转换，不用担心影响容器内部</typeparam>
    /// <param name="targetType"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public object Get(Type targetType)
    {
        var usingGetters = Getters.ToList();
        usingGetters.Reverse();
        SignletonGetter creator;
        creator = usingGetters.FirstOrDefault(g => g.TargetType == targetType);
        var obj = creator.Get();
        return obj;
    }
    /// <summary>
    /// 获取一个目标类型下的所有注册
    /// 返回列表为空代表没有对应记录
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    /// <param name="group"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Target> GetAll<Target>()
    {
        List<Target> objectList = new List<Target>();
        var usingGetters = Getters.ToList();
        //找到注册的所有目标类型
        IEnumerable<SignletonGetter> getters = usingGetters.Where(g => g.TargetType == typeof(Target));
        foreach (var getter in getters)
        {
            objectList.Add((Target)getter.Get());
        }
        return objectList;
    }
    /// <summary>
    /// 获取一个目标类型下的所有注册
    /// 返回列表为空代表没有对应记录
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    /// <param name="group"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<object> GetAll(Type type)
    {
        List<object> objectList = new List<object>();
        //找到注册的所有目标类型
        IEnumerable<SignletonGetter> getters = Getters.Where(g => g.TargetType == type);
        foreach (var getter in getters)
        {
            objectList.Add(getter.Get());
        }
        return objectList;
    }
    public string CheckHint = "";
    public bool Check()
    {
        CheckHint = "";
        bool isOK = true;
        List<Type> Checked = [];
        void TrackOne(Type requiredType)
        {
            Checked.Add(requiredType);
            //列表特殊处理
            if (requiredType.IsGenericType && requiredType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return;
            }

            if (!Getters.Exists(getter => getter.TargetType == requiredType))
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
                        var ctor = requiredType.GetConstructors().First(ctor => ctor.GetParameters().Length == 0);
                        Set(requiredType);
                        CheckHint += $"已自动添加{requiredType}\n";
                    }
                    catch (Exception ex)
                    {
                        CheckHint += $"{requiredType}是没有无参的构造函数\n";
                        isOK = false;
                    }
                }
            }

            var propInfos = requiredType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var propInfo in propInfos)
            {
                if (propInfo.GetCustomAttribute<DIAttribute>() is not null)
                {
                    var attr = propInfo.GetCustomAttribute<DIAttribute>();
                    if (attr.ActualType is null)
                    {
                        if (!Checked.Contains(propInfo.PropertyType))
                            TrackOne(propInfo.PropertyType);
                    }

                }
            }
            //字段依赖注入-------------------------------------------------------------------
            var fieldInfos = requiredType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetCustomAttribute<DIAttribute>() is not null)
                {
                    var attr = fieldInfo.GetCustomAttribute<DIAttribute>();
                    if (attr.ActualType is null)
                    {
                        if (!Checked.Contains(fieldInfo.FieldType))
                            TrackOne(fieldInfo.FieldType);
                    }

                }
            }

        }
        List<Type> requiredTypes = new List<Type>();
        foreach (var getter in Getters.ToArray())
        {
            Type type = getter.ActualType;
            TrackOne(type);
        }
        return isOK;
    }
}
public class SignletonGetter
{
    public ZSingletonDI Container { get; init; }
    public Type ActualType { get; init; }
    public Type TargetType { get; init; }
    object instance;
    public SignletonGetter(Type actualType, Type targetType, ZSingletonDI container)
    {
        ActualType = actualType;
        TargetType = targetType;
        Container = container;
    }
    public SignletonGetter(Type actualType, Type targetType, ZSingletonDI container, object instance)
    {
        ActualType = actualType;
        TargetType = targetType;
        Container = container;
        this.instance = instance;
    }
    public object Get()
    {
        if (instance is not null)
        {
            return instance;
        }
        else
        {
            //搜索无参构造函数
            var ctors = ActualType.GetConstructors();
            var ctor = ctors.First(ctor => ctor.GetParameters().Length == 0);
            //调用构造函数
            List<object> @params = new List<object>();
            instance = ctor.Invoke(@params.ToArray());
            //依赖注入
            InjectDI(instance);
            //执行初始化
            if (instance is ISingletonInit initObj)
            {
                initObj.Init();
            }
            return instance;
        }

    }

    public void InjectDI(object obj)
    {
        var ActualType = obj.GetType();
        //属性依赖注入-------------------------------------------------------------------
        var propInfos = ActualType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var propInfo in propInfos)
        {
            if (propInfo.GetCustomAttribute<DIAttribute>() is not null)
            {
                var attr = propInfo.GetCustomAttribute<DIAttribute>();

                //列表特殊处理
                if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type[] types = propInfo.PropertyType.GenericTypeArguments;
                    if (types.Length == 1)
                    {
                        IList list = (IList)Activator.CreateInstance(propInfo.PropertyType);
                        propInfo.SetValue(obj, list);
                        foreach (var propObj in Container.GetAll(types[0]))
                        {
                            list.Add(propObj);
                        }
                    }
                }
                else
                {
                    if (attr.ActualType is not null)
                    {
                        var propObj = Activator.CreateInstance(attr.ActualType);
                        InjectDI(propObj);
                        //执行初始化
                        if (propObj is ISingletonInit initObj)
                        {
                            initObj.Init();
                        }
                        propInfo.SetValue(obj, propObj);
                    }
                    else
                    {
                        var propObj = Container.Get(propInfo.PropertyType);
                        propInfo.SetValue(obj, propObj);
                    }
                }

            }
        }
        //字段依赖注入-------------------------------------------------------------------
        var fieldInfos = ActualType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var fieldInfo in fieldInfos)
        {
            if (fieldInfo.GetCustomAttribute<DIAttribute>() is not null)
            {

                var attr = fieldInfo.GetCustomAttribute<DIAttribute>();
                //列表特殊处理
                if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type[] types = fieldInfo.FieldType.GenericTypeArguments;
                    if (types.Length == 1)
                    {
                        IList list = (IList)Activator.CreateInstance(fieldInfo.FieldType);
                        fieldInfo.SetValue(obj, list);
                        foreach (var propObj in Container.GetAll(types[0]))
                        {
                            list.Add(propObj);
                        }
                    }
                }
                else
                {
                    if (attr.ActualType is not null)
                    {
                        var propObj = Activator.CreateInstance(attr.ActualType);
                        InjectDI(propObj);
                        //执行初始化
                        if (propObj is ISingletonInit initObj)
                        {
                            initObj.Init();
                        }
                        fieldInfo.SetValue(obj, propObj);
                    }
                    else
                    {
                        var propObj = Container.Get(fieldInfo.FieldType);
                        fieldInfo.SetValue(obj, propObj);
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        return $"SignletonGetter:{this.ActualType}->{this.TargetType}";
    }
}