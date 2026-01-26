using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.Infrastructures;

public class Mapper<F, T> where T :  new()
{
    List<MemberInfo> ignoreMembers = [];
    Dictionary<MemberInfo, MemberInfo> mapDict = [];
    Action<F, T> finalAction;
    static (MemberInfo, Type) ParseExpr<S, M>(Expression<Func<S, M>> toExpr)
    {
        if (toExpr.Body is MemberExpression m)
        {
            if (m.Member is PropertyInfo p)
            {
                return (p, p.PropertyType);
            }
            else if (m.Member is FieldInfo f1)
            {
                return (f1, f1.FieldType);
            }
        }
        else if (toExpr.Body is UnaryExpression u)
        {
            if (u.Operand is MemberExpression m1)
            {
                if (m1.Member is PropertyInfo p1)
                {
                    return (p1, p1.PropertyType);
                }
                else if (m1.Member is FieldInfo f1)
                {
                    return (f1, f1.FieldType);
                }
            }
        }
        throw new InvalidOperationException($"{toExpr}不是合法的成员表达式");
    }
    public Mapper<F, T> IgnoreProp(Expression<Func<F, object>> fromExpr)
    {
        var (m,_)= ParseExpr(fromExpr);
        ignoreMembers.Add(m);
        return this;
    }
    /// <summary>
    /// 指定不同名的映射关系
    /// </summary>
    /// <param name="fromExpr"></param>
    /// <param name="toExpr"></param>
    /// <returns></returns>
    public Mapper<F, T> MapMember(Expression<Func<F, object>> fromExpr, Expression<Func<T, object>> toExpr)
    {
        var (fromMember, _) = ParseExpr(fromExpr);
        var (toMember, _) = ParseExpr(toExpr);
        mapDict.Add(fromMember, toMember);
        return this;
    }
    public Mapper<F, T> Final(Action<F, T> finalAction)
    {
        this.finalAction = finalAction;
        return this;
    }
    public Mapper()
    {
        FindMapMember();
    }
    public void FindMapMember()
    {
        Dictionary<string, MemberInfo> ff = [];
        foreach (var item in typeof(F).GetProperties())
        {
            ff.Add(item.Name.Replace("_", "").ToLower(),item);
        }
        foreach (var item in typeof(F).GetFields())
        {
            ff.Add(item.Name.Replace("_", "").ToLower(), item);
        }
        Dictionary<string, MemberInfo> tt = [];
        foreach (var item in typeof(T).GetProperties())
        {
            tt.Add(item.Name.Replace("_", "").ToLower(), item);
        }
        foreach (var item in typeof(T).GetFields())
        {
            tt.Add(item.Name.Replace("_", "").ToLower(), item);
        }
        foreach (var kv in ff)
        {
            if (tt.ContainsKey(kv.Key))
            {
                mapDict.Add(kv.Value, tt[kv.Key]);
            }
        }
    }
    public object SimpleDefaultMap(object fromObj,Type toType)
    {
        if(fromObj.GetType() == toType)
            return fromObj;
        if(toType == typeof(string))
        {
            return fromObj.ToString();
        }
        if (toType == typeof(Enum))
        {
            return Enum.Parse(toType, fromObj.ToString()); ;
        }
        if (toType == typeof(int))
        {
            return int.Parse(fromObj.ToString()); ;
        }
        return null;
    }
    public void Map(F f, T t)
    {
        foreach (var memberPair in mapDict)
        {
            if (ignoreMembers.Contains(memberPair.Value)) continue;
            object value = null;
            if (memberPair.Key is PropertyInfo fp)
            {
                value = fp.GetValue(f);
            }
            else if (memberPair.Key is FieldInfo ff)
            {
                value = ff.GetValue(f);
            }

            if (memberPair.Value is PropertyInfo tp)
            {
                value= SimpleDefaultMap(value,tp.PropertyType);
                if(value != null)
                    tp.SetValue(t, value);
            }
            else if (memberPair.Value is FieldInfo tf)
            {
                value = SimpleDefaultMap(value, tf.FieldType);
                if (value != null)
                    tf.SetValue(t, value);
            }
        }
        finalAction?.Invoke(f, t);
    }
    public T Map(F f)
    {
        T t = new T();
        Map(f, t);
        return t;
    }
}
public static class ZMapper
{
    public static T To<T>(this object obj) where T : new()
    {
        var mapper = Mappers.FirstOrDefault(kv => kv.Key.from == obj.GetType() && kv.Key.to == typeof(T)).Value;
        if (mapper == null)
            mapper= Define(obj.GetType(), typeof(T));
        dynamic a= mapper;
        var toObj = a.Map((dynamic)obj);
        return (T)toObj;
    }
    public static Dictionary<(Type from, Type to),object> Mappers { get; set; } = new();
    public static Mapper<From, To> Define<From, To>() where To : new()
    {
        Mapper<From, To> mapper = new Mapper<From, To>();
        Mappers.Add((typeof(From), typeof(To)), mapper);
        return mapper;
    }
    public static object Define(Type from,Type to)
    {
        var t = typeof(Mapper<,>).MakeGenericType(from, to);
        var mapper= Activator.CreateInstance(t);
        Mappers.Add((from, to), mapper);
        return mapper;
    }
    public static To Map<From, To>(From from) where To : new()
    {
        var mapper = (Mapper<From, To>)Mappers.First(kv => kv.Key.from == typeof(From) && kv.Key.to == typeof(To)).Value;
        return mapper.Map(from);
    }
    //public static To Map<To>(object from) where To : new()
    //{
    //    return (To)Map(from, from.GetType(), typeof(To));
    //}
    //internal static object Map(object from, Type fromType, Type toType)
    //{
    //    dynamic mapper = Mappers.First(m => m.Key.from == fromType && m.Key.to == toType).Value;
    //    var result = mapper.Map(from);
    //    return result;
    //}
}

//public class Mapper<From, To>: Mapper where To : new()
//{
//    List<string> ignorePropRecords = new List<string>();
//    List<(PropertyInfo fromProp, PropertyInfo toProp)> propMapRelations = new();
//    Action<From, To> finalAction;
//    internal void FindMapRelation()
//    {
//        var fromMembers = typeof(From).GetProperties().ToDictionary(m => m.Name.Replace("_", "").ToLower(), m => m);
//        var toMembers = typeof(To).GetProperties().ToDictionary(m => m.Name.Replace("_", "").ToLower(), m => m);
//        //根据相似命名自动收集映射关系
//        foreach (var kv in toMembers)
//        {
//            PropertyInfo toMemberInfo = kv.Value;
//            if (fromMembers.ContainsKey(kv.Key))
//            {
//                PropertyInfo fromMemberInfo = fromMembers[kv.Key];
//                propMapRelations.Add((fromMemberInfo, toMemberInfo));
//            }
//        }
//    }

//    public Mapper<From, To> IgnoreProp(string propName)
//    {
//        ignorePropRecords.Add(propName);

//        var relation= propMapRelations.First(map=>map.toProp.Name==propName);
//        propMapRelations.Remove(relation);
//        return this;
//    }
//    /// <summary>
//    /// 指定不同名的映射关系
//    /// </summary>
//    /// <param name="fromExpr"></param>
//    /// <param name="toExpr"></param>
//    /// <returns></returns>
//    public Mapper<From, To> MapProp(Expression<Func<From, object>> fromExpr, Expression<Func<To, object>> toExpr)
//    {
//        static (PropertyInfo, Type) ParseExpr<F, T>(Expression<Func<F, T>> toExpr)
//        {
//            if (toExpr.Body is MemberExpression m)
//            {
//                if (m.Member is PropertyInfo p)
//                {
//                    return (p, p.PropertyType);
//                }
//                else if (m.Member is FieldInfo f)
//                {
//                    throw new InvalidOperationException($"{toExpr}不是属性表达式");
//                }
//            }
//            else if (toExpr.Body is UnaryExpression u)
//            {
//                if (u.Operand is MemberExpression m1)
//                {
//                    if (m1.Member is PropertyInfo p1)
//                    {
//                        return (p1, p1.PropertyType);
//                    }
//                    else if (m1.Member is FieldInfo f1)
//                    {
//                        throw new InvalidOperationException($"{toExpr}不是属性表达式");
//                    }
//                }
//            }
//            throw new InvalidOperationException($"{toExpr}不是合法的成员表达式");
//        }
//        var (fromMember, _) = ParseExpr(fromExpr);
//        var (toMember, _) = ParseExpr(toExpr);
//        propMapRelations.Add((fromMember, toMember));
//        return this;
//    }
//    void AutoMap(object fromObj, object toObj)
//    {
//        foreach (var propPair in propMapRelations)
//        {
//            var fromMemberInfo = propPair.fromProp;
//            var toMemberInfo = propPair.toProp;
//            //列表特殊处理
//            if (fromMemberInfo.PropertyType.IsGenericType && toMemberInfo.PropertyType.IsGenericType)
//            {
//                if (fromMemberInfo.PropertyType.IsAssignableTo(typeof(IList)) &&
//                    toMemberInfo.PropertyType.IsAssignableTo(typeof(IList)))
//                {
//                    var innerFromType = fromMemberInfo.PropertyType.GetGenericArguments()[0];
//                    var innerToType = toMemberInfo.PropertyType.GetGenericArguments()[0];

//                    dynamic fromList = fromMemberInfo.GetValue(fromObj);
//                    dynamic toList = Activator.CreateInstance(toMemberInfo.PropertyType);
//                    if (fromList != null)
//                    {
//                        foreach (var fromValue in fromList)
//                        {
//                            if (innerFromType == innerToType)
//                            {
//                                toList.Add(fromValue);
//                            }
//                            else
//                            {
//                                //递归
//                                var toValue = ZMapper.Map(fromValue, innerFromType, innerToType);
//                                toList.Add(toValue);
//                            }
//                        }
//                    }
//                    toMemberInfo.SetValue(toObj, toList);
//                    continue;
//                }
//            }

//            if (toMemberInfo.PropertyType == fromMemberInfo.PropertyType)
//            {
//                toMemberInfo.SetValue(toObj, fromMemberInfo.GetValue(fromObj));
//            }
//            else
//            {
//                var fromValue = fromMemberInfo.GetValue(fromObj);
//                var toValue = ZMapper.Map(fromValue, fromMemberInfo.PropertyType, toMemberInfo.PropertyType);
//                toMemberInfo.SetValue(toObj, toValue);
//            }
//        }
//    }
//    public Mapper<From, To> Final(Action<From, To> finalAction)
//    {
//        this.finalAction = finalAction;
//        return this;
//    }
//    internal override object Map(object from)
//    {
//        var to = new To();
//        AutoMap(from, to);
//        finalAction?.Invoke((From)from, to);
//        return to;
//    }
//    public To Map(From from)
//    {
//        var to = new To();
//        AutoMap(from,to);
//        finalAction?.Invoke(from,to);
//        return to;
//    }
//}
//public static class ZMapper
//{
//    public static T To<T>(this object obj) where T : new()
//    {
//        var mapper = Mappers.First(kv => kv.Key.from == obj.GetType() && kv.Key.to == typeof(T)).Value;
//        var toObj = mapper.Map(obj);
//        return (T)toObj;
//    }
//    public static Dictionary<(Type from, Type to), Mapper> Mappers { get; set; } = new();
//    public static Mapper<From, To> DefineMapper<From, To>() where To : new()
//    {
//        Mapper<From, To> mapper = new Mapper<From, To>();
//        mapper.FindMapRelation();
//        Mappers.Add((typeof(From), typeof(To)), mapper);
//        return mapper;
//    }
//    public static To Map<From, To>(From from) where To : new()
//    {
//        var mapper = (Mapper<From, To>)Mappers.First(kv => kv.Key.from == typeof(From) && kv.Key.to == typeof(To)).Value;
//        return mapper.Map(from);
//    }
//    public static To Map<To>(object from) where To : new()
//    {
//        return (To)Map(from, from.GetType(),typeof(To));
//    }
//    internal static object Map(object from, Type fromType, Type toType)
//    {
//        var mapper = Mappers.First(m => m.Key.from == fromType && m.Key.to == toType).Value;
//        var result = mapper.Map(from);
//        return result;
//    }
//}
